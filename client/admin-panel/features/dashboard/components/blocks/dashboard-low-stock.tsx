import Link from 'next/link';
import { Badge } from '@/components/ui/badge';
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
import { formatNumber, messages } from '@/lib/messages.ar';
import type { LowStockProductItemDto } from '../../types';

const m = messages.dashboard;

interface DashboardLowStockProps {
  products: LowStockProductItemDto[];
}

export function DashboardLowStock({ products }: DashboardLowStockProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{m.lowStockProducts}</CardTitle>
        <CardDescription>{m.lowStockAlert}</CardDescription>
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
                <TableHead>{m.product}</TableHead>
                <TableHead>{m.stock}</TableHead>
                <TableHead>{m.threshold}</TableHead>
                <TableHead className="text-end">{messages.common.edit}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {products.map((product) => (
                <TableRow key={product.id}>
                  <TableCell>
                    <Link
                      href={`/products/${product.id}/view`}
                      className="font-medium hover:underline"
                    >
                      {product.nameAr ?? product.nameEn}
                    </Link>
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline" className="border-amber-500/50 text-amber-700 dark:text-amber-400">
                      {formatNumber(product.stockQuantity)}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {formatNumber(product.lowStockThreshold)}
                  </TableCell>
                  <TableCell className="text-end">
                    <Button variant="ghost" size="sm" asChild>
                      <Link href={`/products/${product.id}/edit`}>
                        {messages.common.edit}
                      </Link>
                    </Button>
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
