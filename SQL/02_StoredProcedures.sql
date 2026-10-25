
use RetailDB;



-- Get Sales Summary
go
create or alter procedure dbo.sp_GetSalesSummary
    @FromDate date,
    @ToDate date,
    @ProductId int = null,
    @CustomerId int = null,
    @CategoryId int = null
as
begin
    set nocount on;

    select
        count(distinct sh.Id) as TotalInvoices,

        isnull(sum(sl.Quantity), 0) as TotalQuantity,

        isnull(sum(sl.Quantity * sl.UnitPrice), 0) as GrossSales,

        isnull(sum(sl.DiscountAmount), 0) as Discount,

        isnull(sum(sl.LineTotal), 0) as NetSales

    from SalesHeaders sh

    inner join SalesLines sl
        on sh.Id = sl.SalesId

    inner join Products p
        on sl.ProductId = p.Id

    where
        sh.IsCancelled = 0

        and sh.SaleDate >= @FromDate

        and sh.SaleDate < dateadd(day, 1, @ToDate)

        and (@ProductId is null or sl.ProductId = @ProductId)

        and (@CustomerId is null or sh.CustomerId = @CustomerId)

        and (@CategoryId is null or p.CategoryId = @CategoryId);
end;
go


EXEC dbo.sp_GetSalesSummary @FromDate = '2026-10-01', @ToDate = '2026-10-31', @ProductId = 1;






--Get Product Sales
go

create or alter procedure dbo.sp_GetProductSales
    @FromDate date,
    @ToDate date,
    @ProductId int = null,
    @CustomerId int = null,
    @CategoryId int = null,
    @PageNumber int = 1,
    @PageSize int = 10,
    @SortBy nvarchar(30) = 'NetSales',
    @SortDirection nvarchar(4) = 'DESC'
as
begin
    set nocount on;

    if @PageNumber < 1 set @PageNumber = 1;
    if @PageSize < 1 set @PageSize = 10;

    if @SortBy not in ('ProductName', 'TotalQuantity', 'GrossSales', 'Discount', 'NetSales')
        set @SortBy = 'NetSales';

    if @SortDirection not in ('ASC', 'DESC')
        set @SortDirection = 'DESC';

    ;with ProductSales as
    (
        select
            p.Id as ProductId,
            p.Name as ProductName,

            sum(sl.Quantity) as TotalQuantity,

            sum(sl.Quantity * sl.UnitPrice) as GrossSales,

            sum(sl.DiscountAmount) as Discount,

            sum(sl.LineTotal) as NetSales

        from SalesHeaders sh

        inner join SalesLines sl
            on sh.Id = sl.SalesId

        inner join Products p
            on sl.ProductId = p.Id

        where
            sh.IsCancelled = 0

            and sh.SaleDate >= @FromDate

            and sh.SaleDate < dateadd(day, 1, @ToDate)

            and (@ProductId is null or sl.ProductId = @ProductId)

            and (@CustomerId is null or sh.CustomerId = @CustomerId)

            and (@CategoryId is null or p.CategoryId = @CategoryId)

        group by p.Id, p.Name
    )
    select
        ProductId,
        ProductName,
        TotalQuantity,
        GrossSales,
        Discount,
        NetSales,
        count(*) over() as TotalRecords
    from ProductSales

    order by
        case when @SortBy = 'ProductName'
              and @SortDirection = 'ASC'
             then ProductName end asc,

        case when @SortBy = 'ProductName'
              and @SortDirection = 'DESC'
             then ProductName end desc,

        case when @SortBy = 'TotalQuantity'
              and @SortDirection = 'ASC'
             then TotalQuantity end asc,

        case when @SortBy = 'TotalQuantity'
              and @SortDirection = 'DESC'
             then TotalQuantity end desc,

        case when @SortBy = 'GrossSales'
              and @SortDirection = 'ASC'
             then GrossSales end asc,

        case when @SortBy = 'GrossSales'
              and @SortDirection = 'DESC'
             then GrossSales end desc,

        case when @SortBy = 'Discount'
              and @SortDirection = 'ASC'
             then Discount end asc,

        case when @SortBy = 'Discount'
              and @SortDirection = 'DESC'
             then Discount end desc,

        case when @SortBy = 'NetSales'
              and @SortDirection = 'ASC'
             then NetSales end asc,

        case when @SortBy = 'NetSales'
              and @SortDirection = 'DESC'
             then NetSales end desc,

        ProductId asc

    offset (@PageNumber - 1) * @PageSize rows
    fetch next @PageSize rows only;
end;
go


EXEC dbo.sp_GetProductSales @FromDate = '2026-10-01', @ToDate = '2026-10-31';

EXEC dbo.sp_GetProductSales @FromDate = '2026-10-01', @ToDate = '2026-10-31', @PageNumber = 1, @PageSize = 2;

EXEC dbo.sp_GetProductSales @FromDate = '2026-10-01', @ToDate = '2026-10-31', @SortBy = 'TotalQuantity', @SortDirection = 'DESC';






-- Get Customer Sales
go

create or alter procedure dbo.sp_GetCustomerSales
    @FromDate date,
    @ToDate date,
    @ProductId int = null,
    @CustomerId int = null,
    @CategoryId int = null,
    @PageNumber int = 1,
    @PageSize int = 10,
    @SortBy nvarchar(30) = 'NetSales',
    @SortDirection nvarchar(4) = 'DESC'
as
begin
    set nocount on;

    if @PageNumber < 1 set @PageNumber = 1;
    if @PageSize < 1 set @PageSize = 10;

    if @SortBy not in ('CustomerName', 'TotalInvoices', 'TotalQuantity',
                       'GrossSales', 'Discount', 'NetSales')
        set @SortBy = 'NetSales';

    if @SortDirection not in ('ASC', 'DESC')
        set @SortDirection = 'DESC';

    ;with CustomerSales as
    (
        select
            c.Id as CustomerId,
            c.Name as CustomerName,

            count(distinct sh.Id) as TotalInvoices,

            sum(sl.Quantity) as TotalQuantity,

            sum(sl.Quantity * sl.UnitPrice) as GrossSales,

            sum(sl.DiscountAmount) as Discount,

            sum(sl.LineTotal) as NetSales

        from SalesHeaders sh

        inner join Customers c
            on sh.CustomerId = c.Id

        inner join SalesLines sl
            on sh.Id = sl.SalesId

        inner join Products p
            on sl.ProductId = p.Id

        where
            sh.IsCancelled = 0

            and sh.SaleDate >= @FromDate

            and sh.SaleDate < dateadd(day, 1, @ToDate)

            and (@ProductId is null or sl.ProductId = @ProductId)

            and (@CustomerId is null or sh.CustomerId = @CustomerId)

            and (@CategoryId is null or p.CategoryId = @CategoryId)

        group by c.Id, c.Name
    )
    select
        CustomerId,
        CustomerName,
        TotalInvoices,
        TotalQuantity,
        GrossSales,
        Discount,
        NetSales,
        count(*) over() as TotalRecords
    from CustomerSales

    order by
        case when @SortBy = 'CustomerName'
              and @SortDirection = 'ASC'
             then CustomerName end asc,

        case when @SortBy = 'CustomerName'
              and @SortDirection = 'DESC'
             then CustomerName end desc,

        case when @SortBy = 'TotalInvoices'
              and @SortDirection = 'ASC'
             then TotalInvoices end asc,

        case when @SortBy = 'TotalInvoices'
              and @SortDirection = 'DESC'
             then TotalInvoices end desc,

        case when @SortBy = 'TotalQuantity'
              and @SortDirection = 'ASC'
             then TotalQuantity end asc,

        case when @SortBy = 'TotalQuantity'
              and @SortDirection = 'DESC'
             then TotalQuantity end desc,

        case when @SortBy = 'GrossSales'
              and @SortDirection = 'ASC'
             then GrossSales end asc,

        case when @SortBy = 'GrossSales'
              and @SortDirection = 'DESC'
             then GrossSales end desc,

        case when @SortBy = 'Discount'
              and @SortDirection = 'ASC'
             then Discount end asc,

        case when @SortBy = 'Discount'
              and @SortDirection = 'DESC'
             then Discount end desc,

        case when @SortBy = 'NetSales'
              and @SortDirection = 'ASC'
             then NetSales end asc,

        case when @SortBy = 'NetSales'
              and @SortDirection = 'DESC'
             then NetSales end desc,

        CustomerId asc

    offset (@PageNumber - 1) * @PageSize rows
    fetch next @PageSize rows only;
end;
go


EXEC dbo.sp_GetCustomerSales @FromDate = '2026-10-01', @ToDate = '2026-10-31';
EXEC dbo.sp_GetCustomerSales @FromDate = '2026-10-01', @ToDate = '2026-10-31', @PageNumber = 1, @PageSize = 2;
EXEC dbo.sp_GetCustomerSales @FromDate = '2026-10-01', @ToDate = '2026-10-31', @SortBy = 'TotalQuantity', @SortDirection = 'DESC';
EXEC dbo.sp_GetCustomerSales  @FromDate = '2026-10-01', @ToDate = '2026-10-31', @CustomerId = 1;