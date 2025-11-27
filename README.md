## 目錄

1. [系統概述](#一系統概述)
2. [資料庫設計](#二資料庫設計)
3. [訂單管理功能](#三訂單管理功能)
4. [檔案清單](#四檔案清單與路徑對照)

---

## 一、系統概述

### 1 專案結構

### 1.1 如何開始

```
1. git clone https://github.com/austinliu0730/OrderMangement.git
2. 建立資料庫 OMS_DB
3. 打開 OMS_DB(資料庫設計).sql -> excute
4. 執行OrderMangement.sln
```

```
OrderMangement/
├── Controllers/               # MVC 控制器
│   └── OrdersController.cs    # 訂單 CRUD 控制器
├── Models/
│   ├── OrderSearchViewModel.cs    # 訂單搜尋與分頁模型
│   └── TotalPriceResult.cs        # 產品統計結果
├── Views/
│   ├── Orders/
│   │   ├── Index.cshtml      # 訂單列表頁
│   │   ├── Create.cshtml     # 新增訂單頁
│   │   └── Edit.cshtml       # 編輯訂單頁
│   └── Shared/
│       └── _Layout.cshtml    # 共用版面配置
├── Helpers/
│   └── ErrorLogger.cs        # 錯誤日誌記錄器
├── Entity Classes/            # EF 實體類別（自動生成）
│   ├── Customer.cs           # 客戶實體
│   ├── Product.cs            # 產品實體
│   └── Order.cs              # 訂單實體
├── OMS_DBModel.edmx          # Entity Framework EDMX 模型
├── Global.asax.cs            # 全域應用程式事件
└── Web.config                # 應用程式組態檔
```

---

## 二、資料庫設計

### 2.1 資料庫設計

```sql
請參考附件OMS_DB.sql

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
```


### 2.2 Stored Procedure

#### Total_Price - 計算產品銷售統計

**輸入參數**：
- `@ProductID int` - 產品編號

**SQL 定義**：

```sql
CREATE PROCEDURE Total_Price
    @ProductID int
AS
BEGIN
    SELECT
        p.ProductName,
        SUM(o.Quantity) AS TotalQuantity,
        SUM(o.Quantity * p.Price) AS TotalPrice
    FROM Orders o
    INNER JOIN Products p ON o.ProductID = p.ProductID
    WHERE o.ProductID = @ProductID
    GROUP BY p.ProductName
END
```

**C# 呼叫方式**（位於 `OrderSearchViewModel.cs`，第 73-87 行）：

```csharp
if (ProductID.HasValue && ProductID.Value > 0)
{
    var productIdParam = new SqlParameter("@ProductID", ProductID.Value);
    var result = db.Database.SqlQuery<TotalPriceResult>(
        "EXEC Total_Price @ProductID",
        productIdParam
    ).FirstOrDefault();

    TotalPriceResult = result;
}
```

---

## 三、訂單管理功能

### 3.1 查詢訂單列表

**功能描述**：
- 依客戶名稱搜尋（模糊比對）
- 依訂單日期搜尋（精確比對）
- 依產品篩選
- 訂單日期排序（升序/降序）
- 分頁顯示（每頁 10 筆）
- 產品銷售統計（選擇特定產品時顯示）

**檔案位置**：
- Controller：`Controllers/OrdersController.cs` → `Index` 方法
- ViewModel：`Models/OrderSearchViewModel.cs` → `SearchOrders` 方法
- View：`Views/Orders/Index.cshtml`

---

### 3.2 新增訂單（AJAX 非同步）

**功能描述**：
- 表單驗證（客戶端 + 伺服器端）
- AJAX 非同步提交
- Anti-Forgery Token 防護
- 成功後導向列表頁

**檔案位置**：
- Controller：`Controllers/OrdersController.cs` → `Create`、`CreateAjax` 方法
- View：`Views/Orders/Create.cshtml`

---

### 3.3 修改訂單

**功能描述**：
- 載入現有訂單資料
- 表單驗證
- 更新資料庫

**檔案位置**：
- Controller：`Controllers/OrdersController.cs` → `Edit` 方法（GET/POST）
- View：`Views/Orders/Edit.cshtml`

---

### 3.4 刪除訂單（AJAX 非同步）

**功能描述**：
- 刪除前確認對話框
- AJAX 非同步刪除
- 錯誤處理
- 成功後重新載入頁面

**檔案位置**：
- Controller：`Controllers/OrdersController.cs` → `DeleteAjax` 方法
- View：`Views/Orders/Index.cshtml`（JavaScript 部分）

---

### 3.5 計算產品銷售總額

**功能描述**：
- 呼叫 Stored Procedure `Total_Price` 計算統計
- 動態顯示產品統計資訊（產品名稱、總數量、總價格）
- 格式化顯示數字（千分位符號）

**檔案位置**：
- ViewModel：`Models/OrderSearchViewModel.cs` → `SearchOrders` 方法
- DTO：`Models/TotalPriceResult.cs`
- View：`Views/Orders/Index.cshtml`

---
