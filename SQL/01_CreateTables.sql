create database RetailDB;

use RetailDB;

-- Task 1: Product Management API

create table Products(
Id int identity(1,1) primary key,
Name nvarchar(150) not null,
Code nvarchar(50) not null unique,
Price decimal(18,2) not null,
Stock int not null default 0,
IsActive bit not null default 1,
CreatedAt datetime2 not null default getutcdate(),
CategoryId int null
);


insert into Products (Name, Code, Price, Stock) values 
('Samsung Galaxy S25 Ultra Pro', 'MOB002', 145300, 70),
('Test Headphones', 'HEAD001', 2500, 20),
('Iphone 17 Pro', 'IP001', 125000, 24),
('Turbo 4 prp', 'Tur_38', 38000, 45),
('redmi note 8', 'redmi_08', 15500, 90);





-- Task 2: Order Management & Inventory

create table Orders(    
Id int identity(1,1) primary key,    
CustomerName nvarchar(150) not null,    
OrderDate datetime2 not null default getutcdate(),    
TotalAmount decimal(18,2) not null default 0,    
Status nvarchar(20) not null default 'Pending'
);



create table OrderItems(    
Id int identity(1,1) primary key,    
OrderId int not null,    
ProductId int not null,    
Quantity int not null,    
UnitPrice decimal(18,2) not null,    
TotalPrice decimal(18,2) not null,  
    
constraint FK_OrderItems_Orders foreign key (OrderId) references Orders(Id),          
constraint FK_OrderItems_Products foreign key (ProductId) references Products(Id),        
constraint UQ_OrderItems_Order_Product unique (OrderId, ProductId)
);

select * from Orders
select * from OrderItems
select * from Products







-- Task 3: Warehouse Stock Transfer API


create table Warehouses(
Id int identity(1,1) primary key,
Name nvarchar(150) not null,
IsActive bit not null default 1
);



create table WarehouseStocks
(
Id int identity(1,1) primary key,
WarehouseId int not null,
ProductId int not null,
Quantity int not null default 0,

constraint FK_WarehouseStocks_Warehouses foreign key (WarehouseId) references Warehouses(Id),
constraint FK_WarehouseStocks_Products foreign key (ProductId) references Products(Id),
constraint UQ_WarehouseStocks_Warehouse_Product unique (WarehouseId, ProductId)
);



create table StockTransfers(
Id int identity(1,1) primary key,
FromWarehouseId int not null,
ToWarehouseId int not null,
TransferDate datetime2 not null default getutcdate(),
Status nvarchar(20) not null default 'Completed',

constraint FK_StockTransfers_FromWarehouse foreign key (FromWarehouseId) references Warehouses(Id),
constraint FK_StockTransfers_ToWarehouse foreign key (ToWarehouseId) references Warehouses(Id)
);



create table StockTransferItems(
Id int identity(1,1) primary key,
StockTransferId int not null,
ProductId int not null,
Quantity int not null,

constraint FK_StockTransferItems_StockTransfers foreign key (StockTransferId) references StockTransfers(Id),
constraint FK_StockTransferItems_Products foreign key (ProductId) references Products(Id),
constraint UQ_StockTransferItems_Transfer_Product unique (StockTransferId, ProductId)
);



select * from Products
SELECT * FROM Warehouses;
SELECT * FROM WarehouseStocks;
SELECT * FROM StockTransfers;
SELECT * FROM StockTransferItems;





-- Test Data Insert for task 3 tables

insert into Warehouses (Name, IsActive)values 
('Main Warehouse', 1),
('Branch Warehouse', 1),
('Closed Warehouse', 0);

insert into WarehouseStocks (WarehouseId, ProductId, Quantity) values 
(1, 1, 70),
(2, 2, 20),
(1,3,24),
(2,4,45),
(1,5,90);







-- Task 4: High-Volume Sales Reporting & Performance API


create table Customers(
Id int identity(1,1) primary key,
Name nvarchar(150) not null,
Phone nvarchar(30) null
);


create table SalesHeaders(
Id int identity(1,1) primary key,
CustomerId int not null,
SaleDate datetime2 not null default getutcdate(),
InvoiceNo nvarchar(50) not null unique,
TotalAmount decimal(18,2) not null,
IsCancelled bit not null default 0,

constraint FK_SalesHeaders_Customers foreign key (CustomerId) references Customers(Id)
);



create table SalesLines(
Id int identity(1,1) primary key,
SalesId int not null,
ProductId int not null,
Quantity int not null,
UnitPrice decimal(18,2) not null,
DiscountAmount decimal(18,2) not null default 0,
LineTotal decimal(18,2) not null,


constraint FK_SalesLines_SalesHeaders foreign key (SalesId) references SalesHeaders(Id),
constraint FK_SalesLines_Products foreign key (ProductId) references Products(Id)
);

select * from Customers;
select * from SalesHeaders;
select * from SalesLines;





-- Test Data Insert for task 04


begin transaction;
begin try

-- 1. Insert sample customers
insert into Customers (Name, Phone) values
('Rahim Ahmed', '01711111111'),
('Karim Hasan', '01822222222'),
('Nusrat Jahan', '01933333333');


-- 2. Get customer IDs
declare @Customer1 int;
declare @Customer2 int;
declare @Customer3 int;

select @Customer1 = Id from Customers
where Phone = '01711111111';

select @Customer2 = Id from Customers
where Phone = '01822222222';

select @Customer3 = Id from Customers
where Phone = '01933333333';



-- 3. Insert sales headers
insert into SalesHeaders (CustomerId, SaleDate, InvoiceNo, TotalAmount, IsCancelled) values
(@Customer1, '2026-10-20', 'INV-1001', 0, 0),
(@Customer2, '2026-10-21', 'INV-1002', 0, 0),
(@Customer3, '2026-10-22', 'INV-1003', 0, 0),
(@Customer1, '2026-10-23', 'INV-1004', 0, 1);



-- 4. Get invoice IDs
declare @Sale1 int, @Sale2 int, @Sale3 int, @Sale4 int;

select @Sale1 = Id from SalesHeaders where InvoiceNo = 'INV-1001';
select @Sale2 = Id from SalesHeaders where InvoiceNo = 'INV-1002';
select @Sale3 = Id from SalesHeaders where InvoiceNo = 'INV-1003';
select @Sale4 = Id from SalesHeaders where InvoiceNo = 'INV-1004';


-- 5. Insert sales lines using database product prices
insert into SalesLines (SalesId, ProductId, Quantity, UnitPrice, DiscountAmount, LineTotal)
select @Sale1, Id, 2, Price, 1000, (2 * Price) - 1000
from Products
where Id = 1;

insert into SalesLines (SalesId, ProductId, Quantity, UnitPrice, DiscountAmount, LineTotal)
select @Sale1, Id, 1, Price, 200, Price - 200
from Products
where Id = 2;

insert into SalesLines (SalesId, ProductId, Quantity, UnitPrice, DiscountAmount, LineTotal)
select @Sale2, Id, 1, Price, 500, Price - 500
from Products
where Id = 3;

insert into SalesLines (SalesId, ProductId, Quantity, UnitPrice, DiscountAmount, LineTotal)
select @Sale3, Id, 3, Price, 1500, (3 * Price) - 1500
from Products
where Id = 1;

insert into SalesLines (SalesId, ProductId, Quantity, UnitPrice, DiscountAmount, LineTotal)
select @Sale4, Id, 1, Price, 0, Price
from Products
where Id = 3;



-- 6. Calculate invoice totals
update sh
set sh.TotalAmount = totals.Total
from SalesHeaders sh
inner join (
    select SalesId, sum(LineTotal) as Total
    from SalesLines
    group by SalesId
) totals 
    on sh.Id = totals.SalesId
where sh.InvoiceNo in (
    'INV-1001',
    'INV-1002',
    'INV-1003',
    'INV-1004'
);

commit transaction;
end try

begin catch
    rollback transaction;
    throw;
end catch;




select * from Customers;
select * from SalesHeaders;
select * from SalesLines;

