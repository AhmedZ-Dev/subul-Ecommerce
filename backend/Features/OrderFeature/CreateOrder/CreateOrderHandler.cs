using System.Text.Json;
using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.OrderFeature.CreateOrder;

public class CreateOrderHandler(AppDbContext context)
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    private static readonly HashSet<string> ValidPaymentMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "cod", "bank_transfer", "online"
    };

    public async Task<Result<CreateOrderResponse>> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var sessionId = NormalizeSession(command.SessionId);
        if (sessionId is null)
            return Result<CreateOrderResponse>.Failure("Cart session is required");

        var paymentMethod = command.PaymentMethod.Trim().ToLowerInvariant();
        if (!ValidPaymentMethods.Contains(paymentMethod))
            return Result<CreateOrderResponse>.Failure("Invalid payment method");

        if (command.UserId is not null)
        {
            var userExists = await context.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
            if (!userExists)
                return Result<CreateOrderResponse>.Failure("User not found");
        }

        var shipping = await ResolveShippingAsync(command, cancellationToken);
        if (shipping.Error is not null)
            return Result<CreateOrderResponse>.Failure(shipping.Error);

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Variant)
            .FirstOrDefaultAsync(
                c => c.SessionId == sessionId || (command.UserId != null && c.UserId == command.UserId),
                cancellationToken);

        if (cart is null || cart.CartItems.Count == 0)
            return Result<CreateOrderResponse>.Failure("Cart is empty");

        foreach (var item in cart.CartItems)
        {
            if (!string.Equals(item.Product.Status, "active", StringComparison.OrdinalIgnoreCase))
                return Result<CreateOrderResponse>.Failure($"Product '{item.Product.NameEn}' is not available");

            if (item.VariantId is not null && item.Variant is not null && !item.Variant.IsActive)
                return Result<CreateOrderResponse>.Failure($"Product variant for '{item.Product.NameEn}' is not available");

            var availableStock = item.Variant?.StockQuantity ?? item.Product.StockQuantity;
            if (item.Quantity > availableStock)
                return Result<CreateOrderResponse>.Failure($"Insufficient stock for '{item.Product.NameEn}'");
        }

        var subtotal = cart.CartItems.Sum(ci =>
        {
            var unitPrice = ci.UnitPrice ?? ResolveUnitPrice(ci.Product, ci.Variant);
            return unitPrice * ci.Quantity;
        });

        var (shippingAmount, zoneId, shippingError) = await CalculateShippingAsync(
            subtotal,
            shipping.ShippingZoneId,
            shipping.ShippingGovernorate,
            cancellationToken);

        if (shippingError is not null)
            return Result<CreateOrderResponse>.Failure(shippingError);

        var discountAmount = 0m;
        var taxAmount = 0m;
        var total = subtotal - discountAmount + shippingAmount + taxAmount;
        var currency = cart.CartItems.First().Product.Currency;
        var now = DateTime.Now;
        var orderNumber = await GenerateOrderNumberAsync(cancellationToken);

        var order = new Order
        {
            UserId = command.UserId,
            OrderNumber = orderNumber,
            Status = "pending",
            PaymentStatus = "pending",
            FulfillmentStatus = "unfulfilled",
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            ShippingAmount = shippingAmount,
            TaxAmount = taxAmount,
            Total = total,
            Currency = currency,
            CouponCode = command.CouponCode?.Trim(),
            ShippingFirstName = shipping.ShippingFirstName,
            ShippingLastName = shipping.ShippingLastName,
            ShippingPhone = shipping.ShippingPhone,
            ShippingAddress1 = shipping.ShippingAddress1,
            ShippingAddress2 = shipping.ShippingAddress2,
            ShippingCity = shipping.ShippingCity,
            ShippingGovernorate = shipping.ShippingGovernorate,
            ShippingCountry = shipping.ShippingCountry,
            ShippingZoneId = zoneId ?? shipping.ShippingZoneId,
            PaymentMethod = paymentMethod,
            CustomerNotes = command.CustomerNotes?.Trim(),
            IpAddress = command.IpAddress?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        foreach (var cartItem in cart.CartItems)
        {
            var unitPrice = cartItem.UnitPrice ?? ResolveUnitPrice(cartItem.Product, cartItem.Variant);
            var lineTotal = unitPrice * cartItem.Quantity;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                VariantId = cartItem.VariantId,
                ProductName = cartItem.Product.NameEn,
                Sku = cartItem.Variant?.Sku ?? cartItem.Product.Sku,
                Quantity = cartItem.Quantity,
                UnitPrice = unitPrice,
                CompareAtPrice = cartItem.Variant?.CompareAtPrice ?? cartItem.Product.CompareAtPrice,
                DiscountAmount = 0,
                TotalPrice = lineTotal,
                WarrantyMonths = cartItem.Product.WarrantyMonths,
                RequiresShipping = cartItem.Product.RequiresShipping,
                CreatedAt = now
            });

            if (cartItem.Variant is not null)
                cartItem.Variant.StockQuantity -= cartItem.Quantity;
            else
                cartItem.Product.StockQuantity -= cartItem.Quantity;

            cartItem.Product.TotalSold += cartItem.Quantity;
            cartItem.Product.UpdatedAt = now;
        }

        order.OrderStatusHistories.Add(new OrderStatusHistory
        {
            FromStatus = null,
            ToStatus = "pending",
            ChangedByType = "customer",
            Note = "Order placed",
            CreatedAt = now
        });

        context.Orders.Add(order);
        context.CartItems.RemoveRange(cart.CartItems);
        cart.UpdatedAt = now;

        await context.SaveChangesAsync(cancellationToken);

        var items = order.OrderItems.Select(oi => new CreateOrderItemResponse(
            oi.Id,
            oi.ProductId,
            oi.VariantId,
            oi.ProductName,
            oi.Sku,
            oi.Quantity,
            oi.UnitPrice,
            oi.TotalPrice)).ToList();

        var response = new CreateOrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.Status,
            order.PaymentStatus,
            order.FulfillmentStatus,
            order.Subtotal,
            order.DiscountAmount,
            order.ShippingAmount,
            order.TaxAmount,
            order.Total,
            order.Currency,
            order.ShippingFirstName,
            order.ShippingLastName,
            order.ShippingPhone,
            order.ShippingAddress1,
            order.ShippingCity,
            order.ShippingGovernorate,
            order.ShippingCountry,
            order.ShippingZoneId,
            order.PaymentMethod,
            order.CreatedAt,
            items);

        return Result<CreateOrderResponse>.Success(response);
    }

    private static string? NormalizeSession(string? sessionId) =>
        string.IsNullOrWhiteSpace(sessionId) ? null : sessionId.Trim();

    private static decimal ResolveUnitPrice(Product product, ProductVariant? variant) =>
        variant?.Price ?? product.Price;

    private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
            var exists = await context.Orders.AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);
            if (!exists)
                return orderNumber;
        }

        return $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }

    private async Task<(decimal Amount, long? ZoneId, string? Error)> CalculateShippingAsync(
        decimal subtotal,
        long? shippingZoneId,
        string? governorate,
        CancellationToken cancellationToken)
    {
        ShippingZone? zone = null;

        if (shippingZoneId is not null)
        {
            zone = await context.ShippingZones
                .Include(z => z.ShippingRates)
                .FirstOrDefaultAsync(z => z.Id == shippingZoneId && z.IsActive, cancellationToken);

            if (zone is null)
                return (0, null, "Shipping zone not found");
        }
        else if (!string.IsNullOrWhiteSpace(governorate))
        {
            var normalizedGovernorate = governorate.Trim();
            var zones = await context.ShippingZones
                .Include(z => z.ShippingRates)
                .Where(z => z.IsActive)
                .ToListAsync(cancellationToken);

            zone = zones.FirstOrDefault(z => ZoneCoversGovernorate(z.Governorates, normalizedGovernorate));

            if (zone is null)
                return (0, null, "No shipping zone found for governorate");
        }
        else
        {
            return (0, null, "Shipping zone or governorate is required");
        }

        var rate = zone.ShippingRates
            .Where(r => r.IsActive && string.Equals(r.RateType, "flat", StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.Price)
            .FirstOrDefault();

        if (rate is null)
            return (0, zone.Id, "No active shipping rate found for zone");

        if (rate.FreeShippingThreshold is not null && subtotal >= rate.FreeShippingThreshold.Value)
            return (0, zone.Id, null);

        if (rate.MinOrderValue is not null && subtotal < rate.MinOrderValue.Value)
            return (0, zone.Id, $"Minimum order value for shipping is {rate.MinOrderValue.Value}");

        if (rate.MaxOrderValue is not null && subtotal > rate.MaxOrderValue.Value)
            return (0, zone.Id, $"Maximum order value for shipping is {rate.MaxOrderValue.Value}");

        return (rate.Price, zone.Id, null);
    }

    private static bool ZoneCoversGovernorate(string? governoratesJson, string governorate)
    {
        if (string.IsNullOrWhiteSpace(governoratesJson))
            return false;

        try
        {
            var governorates = JsonSerializer.Deserialize<List<string>>(governoratesJson);
            return governorates?.Any(g =>
                string.Equals(g.Trim(), governorate, StringComparison.OrdinalIgnoreCase)) ?? false;
        }
        catch (JsonException)
        {
            return governoratesJson.Contains(governorate, StringComparison.OrdinalIgnoreCase);
        }
    }

    private async Task<(string? Error, string? ShippingFirstName, string? ShippingLastName, string? ShippingPhone,
        string? ShippingAddress1, string? ShippingAddress2, string? ShippingCity, string? ShippingGovernorate,
        string ShippingCountry, long? ShippingZoneId)> ResolveShippingAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (command.AddressId is not null)
        {
            if (command.UserId is null)
                return ("AddressId requires a registered user", null, null, null, null, null, null, null, "Iraq", null);

            var address = await context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == command.AddressId && a.UserId == command.UserId, cancellationToken);

            if (address is null)
                return ("Address not found", null, null, null, null, null, null, null, "Iraq", null);

            return (null,
                address.FirstName,
                address.LastName,
                address.Phone,
                address.Address1,
                address.Address2,
                address.City,
                address.Governorate,
                address.Country,
                command.ShippingZoneId);
        }

        if (command.UserId is null)
        {
            if (string.IsNullOrWhiteSpace(command.ShippingFirstName))
                return ("Shipping first name is required for guest checkout", null, null, null, null, null, null, null, "Iraq", null);

            if (string.IsNullOrWhiteSpace(command.ShippingPhone))
                return ("Shipping phone is required for guest checkout", null, null, null, null, null, null, null, "Iraq", null);

            if (string.IsNullOrWhiteSpace(command.ShippingAddress1))
                return ("Shipping address is required for guest checkout", null, null, null, null, null, null, null, "Iraq", null);

            if (string.IsNullOrWhiteSpace(command.ShippingCity))
                return ("Shipping city is required for guest checkout", null, null, null, null, null, null, null, "Iraq", null);

            if (string.IsNullOrWhiteSpace(command.ShippingGovernorate))
                return ("Shipping governorate is required for guest checkout", null, null, null, null, null, null, null, "Iraq", null);
        }
        else
        {
            var hasInlineShipping = !string.IsNullOrWhiteSpace(command.ShippingAddress1)
                                    && !string.IsNullOrWhiteSpace(command.ShippingPhone);

            if (!hasInlineShipping)
                return ("Shipping information or addressId is required", null, null, null, null, null, null, null, "Iraq", null);
        }

        var country = string.IsNullOrWhiteSpace(command.ShippingCountry) ? "Iraq" : command.ShippingCountry.Trim();

        return (null,
            command.ShippingFirstName?.Trim(),
            command.ShippingLastName?.Trim(),
            command.ShippingPhone?.Trim(),
            command.ShippingAddress1?.Trim(),
            command.ShippingAddress2?.Trim(),
            command.ShippingCity?.Trim(),
            command.ShippingGovernorate?.Trim(),
            country,
            command.ShippingZoneId);
    }
}
