CREATE DATABASE productdb;
USE productdb;

CREATE TABLE Products (
    Id INT PRIMARY KEY identity (1,1),
    Name VARCHAR(100),
    Price DECIMAL(10,2),
    Quantity INT
);

select * from products