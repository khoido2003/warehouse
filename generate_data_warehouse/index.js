const { Faker, en } = require("@faker-js/faker");
const sql = require("mssql");

// Check current port instance running on
//EXEC xp_readerrorlog 0, 1, N'Server is listening on';

// Initialize Faker
const faker = new Faker({ locale: [en] });

// SQL Server configuration

const config = {
  user: "sa",
  password: "1",
  server: "localhost",
  port: 63919,
  database: "master",
  options: {
    encrypt: true,
    trustServerCertificate: true,
  },
};

async function checkAndCreateDatabase() {
  try {
    const pool = await sql.connect(config);
    const result = await pool.request().query(`
      IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'warehouse_db')
      BEGIN
        CREATE DATABASE warehouse_db;
        SELECT 'Database created' AS Result;
      END
      ELSE
      BEGIN
        SELECT 'Database already exists' AS Result;
      END
    `);
    console.log(result.recordset[0].Result);
    await pool.close();
  } catch (err) {
    console.error("Error:", err);
  }
}

// Helper function to execute SQL queries
async function executeQuery(query) {
  const pool = await sql.connect(config);
  await pool.request().query(`USE warehouse_db; ${query}`);
  await pool.close();
}

// Helper function to generate random integers
function getRandomInt(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

// 1. Create Tables
async function createTables() {
  const createTablesQuery = `
    IF OBJECT_ID('Fact_Sale') IS NOT NULL DROP TABLE Fact_Sale;
    IF OBJECT_ID('Fact_Inventory') IS NOT NULL DROP TABLE Fact_Inventory;
    IF OBJECT_ID('Dim_Time') IS NOT NULL DROP TABLE Dim_Time;
    IF OBJECT_ID('Dim_Store') IS NOT NULL DROP TABLE Dim_Store;
    IF OBJECT_ID('Dim_City') IS NOT NULL DROP TABLE Dim_City;
    IF OBJECT_ID('Dim_Customer') IS NOT NULL DROP TABLE Dim_Customer;
    IF OBJECT_ID('Dim_Product') IS NOT NULL DROP TABLE Dim_Product;

    CREATE TABLE Dim_City (
      City_id INT PRIMARY KEY IDENTITY(1,1),
      City_name VARCHAR(25),
      Office_addr VARCHAR(255),
      States VARCHAR(50)
    );

    CREATE TABLE Dim_Store (
      Store_id INT PRIMARY KEY IDENTITY(1,1),
      City_id INT FOREIGN KEY REFERENCES Dim_City(City_id),
      Phone VARCHAR(50) 
    );

    CREATE TABLE Dim_Customer (
      Customer_id INT PRIMARY KEY IDENTITY(1,1),
      Customer_name VARCHAR(150),
      City_id INT FOREIGN KEY REFERENCES Dim_City(City_id),
      Travel VARCHAR(50),
      Post VARCHAR(50)
    );

    CREATE TABLE Dim_Product (
      Product_id INT PRIMARY KEY IDENTITY(1,1),
      Description VARCHAR(255),
      Size VARCHAR(10),
      Weight VARCHAR(10),
      Price FLOAT
    );

    CREATE TABLE Dim_Time (
      Time_id INT PRIMARY KEY IDENTITY(1,1),
      Day INT,
      Month INT,
      Quarter INT,
      Year INT
    );

    CREATE TABLE Fact_Sale (
      Time_id INT FOREIGN KEY REFERENCES Dim_Time(Time_id),
      Customer_id INT FOREIGN KEY REFERENCES Dim_Customer(Customer_id),
      Product_id INT FOREIGN KEY REFERENCES Dim_Product(Product_id),
      Unit_sold INT,
      Total_amount FLOAT
    );

    CREATE TABLE Fact_Inventory (
      Time_id INT FOREIGN KEY REFERENCES Dim_Time(Time_id),
      Product_id INT FOREIGN KEY REFERENCES Dim_Product(Product_id),
      Store_id INT FOREIGN KEY REFERENCES Dim_Store(Store_id),
      Quantity INT
    );
  `;
  await executeQuery(createTablesQuery);
  console.log("Tables created successfully.");
}

// 2. Generate Data for Dim_City (50 cities)
async function generateDimCity() {
  const pool = await sql.connect(config);
  for (let i = 0; i < 50; i++) {
    const cityName = faker.location.city();
    const officeAddr = faker.location.streetAddress();
    const state = faker.location.state();
    const query = `
      INSERT INTO Dim_City (City_name, Office_addr, States)
      VALUES (@cityName, @officeAddr, @state)
    `;
    const request = pool.request();
    request.input("cityName", sql.VarChar(25), cityName);
    request.input("officeAddr", sql.VarChar(255), officeAddr);
    request.input("state", sql.VarChar(50), state);
    await request.query(`USE warehouse_db; ${query}`);
  }
  await pool.close();
  console.log("Dim_City data generated.");
}

// 3. Generate Data for Dim_Store (200 stores)
async function generateDimStore() {
  const pool = await sql.connect(config);
  for (let i = 0; i < 200; i++) {
    const cityId = getRandomInt(1, 50); // 50 cities
    const phone = faker.phone.number();
    const query = `
      INSERT INTO Dim_Store (City_id, Phone)
      VALUES (${cityId}, '${phone}')
    `;
    await pool.request().query(`USE warehouse_db; ${query}`);
  }
  await pool.close();
  console.log("Dim_Store data generated.");
}

// 4. Generate Data for Dim_Customer (5,000 customers)
async function generateDimCustomer() {
  const pool = await sql.connect(config);
  for (let i = 0; i < 5000; i++) {
    const customerName = faker.person.fullName();
    const cityId = getRandomInt(1, 50);
    const isTourist = Math.random() > 0.5;
    const isPostal = Math.random() > 0.5;
    const travel = isTourist ? faker.person.fullName() : null;
    const post = isPostal ? faker.location.streetAddress() : null;
    const query = `
      INSERT INTO Dim_Customer (Customer_name, City_id, Travel, Post)
      VALUES (@customerName, @cityId, @travel, @post)
    `;
    const request = pool.request();
    request.input("customerName", sql.VarChar(100), customerName);
    request.input("cityId", sql.Int, cityId);
    request.input("travel", sql.VarChar(50), travel);
    request.input("post", sql.VarChar(50), post);
    await request.query(`USE warehouse_db; ${query}`);
  }
  await pool.close();
  console.log("Dim_Customer data generated.");
}

// 5. Generate Data for Dim_Product (500 products)
async function generateDimProduct() {
  const pool = await sql.connect(config);
  for (let i = 0; i < 500; i++) {
    const description = faker.commerce.productDescription();
    const size = ["Small", "Medium", "Large"][getRandomInt(0, 2)];
    const weight = `${getRandomInt(1, 10)}kg`;
    const price = parseFloat(faker.commerce.price({ min: 5, max: 100 }));

    const query = `
      USE warehouse_db;
      INSERT INTO Dim_Product (Description, Size, Weight, Price)
      VALUES (@description, @size, @weight, @price)
    `;
    const request = pool.request();
    request.input("description", sql.VarChar(255), description);
    request.input("size", sql.VarChar(10), size);
    request.input("weight", sql.VarChar(10), weight);
    request.input("price", sql.Float, price);
    await request.query(query);
  }
  await pool.close();
  console.log("Dim_Product data generated.");
}

// 6. Generate Data for Dim_Time (5,000 days, ~14 years from 2010 to 2024)
async function generateDimTime() {
  const pool = await sql.connect(config);
  let startDate = new Date(2010, 0, 1); // Start from Jan 1, 2010
  for (let i = 0; i < 5000; i++) {
    const day = startDate.getDate();
    const month = startDate.getMonth() + 1;
    const year = startDate.getFullYear();
    const quarter = Math.ceil(month / 3);
    const query = `
      INSERT INTO Dim_Time (Day, Month, Quarter, Year)
      VALUES (${day}, ${month}, ${quarter}, ${year})
    `;
    await pool.request().query(`USE warehouse_db; ${query}`);
    startDate.setDate(startDate.getDate() + 1); // Increment by 1 day
  }
  await pool.close();
  console.log("Dim_Time data generated.");
}

// 7. Generate Data for Fact_Sale (10,000 sales)
async function generateFactSale() {
  const pool = await sql.connect(config);
  for (let i = 0; i < 10000; i++) {
    const timeId = getRandomInt(1, 5000);
    const customerId = getRandomInt(1, 5000);
    const productId = getRandomInt(1, 500);
    const unitSold = getRandomInt(1, 50);
    // Fetch price to calculate total_amount
    const priceResult = await pool.request().query(`
      USE warehouse_db;
      SELECT Price FROM Dim_Product WHERE Product_id = ${productId}
    `);
    const price = priceResult.recordset[0].Price;
    const totalAmount = unitSold * price;
    const query = `
      INSERT INTO Fact_Sale (Time_id, Customer_id, Product_id, Unit_sold, Total_amount)
      VALUES (${timeId}, ${customerId}, ${productId}, ${unitSold}, ${totalAmount})
    `;
    await pool.request().query(`USE warehouse_db; ${query}`);
  }
  await pool.close();
  console.log("Fact_Sale data generated.");
}

// 8. Generate Data for Fact_Inventory (10,000 inventory records)
async function generateFactInventory() {
  const pool = await sql.connect(config);
  for (let i = 0; i < 10000; i++) {
    const timeId = getRandomInt(1, 5000);
    const productId = getRandomInt(1, 500);
    const storeId = getRandomInt(1, 200);
    const quantity = getRandomInt(0, 1000);
    const query = `
      INSERT INTO Fact_Inventory (Time_id, Product_id, Store_id, Quantity)
      VALUES (${timeId}, ${productId}, ${storeId}, ${quantity})
    `;
    await pool.request().query(`USE warehouse_db; ${query}`);
  }
  await pool.close();
  console.log("Fact_Inventory data generated.");
}

async function main() {
  try {
    await checkAndCreateDatabase();
    await createTables();
    await generateDimCity();
    await generateDimStore();
    await generateDimCustomer();
    await generateDimProduct();
    await generateDimTime();
    await generateFactSale();
    await generateFactInventory();
    console.log("Data generation completed successfully!");
  } catch (err) {
    console.error("Error during data generation:", err);
  } finally {
    await sql.close();
  }
}

main();
