import Link from 'next/link';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { formatCurrency, formatNumber, messages } from '@/lib/messages.ar';
import type { TopSellingProductItemDto } from '../../types';

const m = messages.dashboard;

interface DashboardTopSellersProps {
  products: TopSellingProductItemDto[];
}

export function DashboardTopSellers({ products }: DashboardTopSellersProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{m.topSellingProducts}</CardTitle>
        <CardDescription>{m.topSelling}</CardDescription>
        <CardAction>
          <Button variant="outline" size="sm" asChild>
            <Link href="/products">{m.viewAllProducts}</Link>
          </Button>
        </CardAction>
      </CardHeader>
      <CardContent>
        {products.length === 0 ? (
          <p className="text-muted-foreground py-8 text-center text-sm">{m.noData}</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-10">#</TableHead>
                <TableHead>{m.product}</TableHead>
                <TableHead>{m.sold}</TableHead>
                <TableHead>{m.price}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {products.map((product, index) => (
                <TableRow key={product.id}>
                  <TableCell className="text-muted-foreground tabular-nums">
                    {index + 1}
                  </TableCell>
                  <TableCell>
                    <Link
                      href={`/products/${product.id}/view`}
                      className="font-medium hover:underline"
                    >
                      {product.nameAr ?? product.nameEn}
                    </Link>
                  </TableCell>
                  <TableCell className="tabular-nums">
                    {formatNumber(product.totalSold)}
                    <span className="text-muted-foreground ms-1 text-xs">
                      {m.unitsSold}
                    </span>
                  </TableCell>
                  <TableCell className="tabular-nums">
                    {formatCurrency(product.price, product.currency)}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
}
