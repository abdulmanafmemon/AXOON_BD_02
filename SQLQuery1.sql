CREATE DATABASE productdb;
USE productdb;

CREATE TABLE Products (
    Id INT PRIMARY KEY identity (1,1),
    Name VARCHAR(100),
    Price DECIMAL(10,2),
    Quantity INT
);
DBCC CHECKIDENT ('Products', RESEED, 0);

select * from products
delete from products

ALTER TABLE Users ADD Role VARCHAR(50) NOT NULL DEFAULT 'user';

SELECT * FROM dbo.CurrencyRates;
