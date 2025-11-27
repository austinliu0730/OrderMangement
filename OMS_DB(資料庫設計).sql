---建立資料表---
CREATE TABLE Products
(
	ProductID int Primary key identity(1,1) NOT NULL,
	ProductName nvarchar(50) NOT NULL,
	Price decimal (10,2) NOT NULL
)

CREATE TABLE Customers
(
	CustomerID int Primary key identity(1,1) NOT NULL,
	CustomerName nvarchar(50) NOT NULL, 
	Email nvarchar (50) NOT NULL,
	Phone nvarchar (20) NOT NULL
)

CREATE TABLE Orders
(
	OrderID int Primary key identity(1,1) NOT NULL,
	ProductID int NOT NULL,
	CustomerID int NOT NULL,
	Quantity int NOT NULL,
	OrderDate DateTime NOT NULL,
	FOREIGN KEY(ProductID)  REFERENCES Products(ProductID),
	FOREIGN KEY(CustomerID)  REFERENCES Customers(CustomerID)
)

---插入測試資料---

INSERT INTO Products(ProductName,Price)
VALUES('筆記型電腦',35000.00),
	  ('滑鼠',500.00),
	  ('鍵盤',800.00),
	  ('螢幕',5000.00),
	  ('耳機',1200.00);

INSERT INTO Customers(CustomerName,Email,Phone)
VALUES('王小明','xiaoming@example.com','0912345678'),
	  ('李小華','lihua@example.com','0922333444'),
	  ('陳大同','dahong@example.com','0933222111');

INSERT INTO Orders(ProductID,CustomerID,Quantity,OrderDate)
VALUES(1,1,2,'2025-11-01'),
	  (2,1,5,'2025-11-02'),
	  (3,2,1,'2025-11-03'),
	  (4,3,3,'2025-11-03'),
	  (5,2,2,'2025-11-04');

---分頁需求新增資料---

INSERT INTO Orders(ProductID,CustomerID,Quantity,OrderDate)
VALUES(1,2,3,'2025-11-05'),
      (2,3,7,'2025-11-06'),
      (3,1,4,'2025-11-07'),
      (4,2,9,'2025-11-08'),
      (5,3,2,'2025-11-09'),
      (1,1,6,'2025-11-10'),
      (2,2,5,'2025-11-11'),
      (3,3,8,'2025-11-12'),
      (4,1,1,'2025-11-13'),
      (5,2,4,'2025-11-14'),
      (1,3,7,'2025-11-15'),
      (2,1,3,'2025-11-16'),
      (3,2,6,'2025-11-17'),
      (4,3,2,'2025-11-18'),
      (5,1,9,'2025-11-19'),
      (1,2,5,'2025-11-20'),
      (2,3,4,'2025-11-21'),
      (3,1,8,'2025-11-22'),
      (4,2,3,'2025-11-23'),
      (5,3,6,'2025-11-24');

---檢查資料內容---

SELECT * FROM Orders;
SELECT * FROM Customers;
SELECT * FROM Orders;


SELECT o.ProductID,sum(o.Quantity),SUM(p.Price) FROM 
 Orders o 
 JOIN Products p
 ON p.ProductID = o.ProductID
 GROUP BY o.ProductID,p.Price,o.Quantity;

