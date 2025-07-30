
CREATE DATABASE HotelAccomodationSystem;

Use HotelAccomodationSystem;

drop database HotelAccomodationSystem;
-------------------------------------------------------------------------------------------------------

Drop table Login;
--Login--
CREATE TABLE Login (
    ID varchar(50),
    Username varchar(50),
    Password varchar(50),
	Userrole varchar(20),
	EmpID VARCHAR(10) Foreign Key  References Employee(EmployeeID)

);

ALTER TABLE Login ADD ProfileImagePath VARCHAR(255);


INSERT INTO Login (ID, Username, Password,Userrole,EmpID)
VALUES ('1', 'HASNIBM222', 'HAS22222','Admin','E006'),
('2', 'CASNIBM222', 'CAS22222','User','E007');

UPDATE Login
SET ProfileImagePath = 'Images/HASNIBM222.png'
WHERE Username = 'HASNIBM222';

UPDATE Login
SET ProfileImagePath = 'Images/CASNIBM222.png'
WHERE Username = 'CASNIBM222';


----------------------------------------------------------------------------------------------------------
Drop table room;
--Room--
CREATE TABLE Room (
    RoomID VARCHAR(10) PRIMARY KEY,
    RoomType VARCHAR(30),
    BedType VARCHAR(30),
    Price DECIMAL(8,2),
    CreateDate DATE,
	UpdatedDate DATE NULL
);
SELECT DISTINCT RoomID FROM Room WHERE RoomType = @RoomType AND BedType = @BedType
INSERT INTO Room (RoomID, RoomType, BedType, Price, CreateDate, UpdatedDate)
VALUES 
('R001', 'AC-Single', 'Queen', 2000.00, '2024-05-31', '2024-05-31'),
('R002', 'AC-Single', 'Queen', 2000.00, '2024-05-31', '2024-05-31'),
('R003', 'NONAC-Single', 'Queen', 1500.00, '2024-05-31', '2024-05-31'),
('R004', 'NONAC-Single', 'Queen', 1500.00, '2024-05-31', '2024-05-31'),
('R005', 'AC-Double', 'King', 2500.00, '2024-05-31','2024-05-31'),
('R006', 'AC-Double', 'King', 2500.00, '2024-05-31','2024-05-31'),
('R007', 'NONAC-Double', 'King', 2200.00, '2024-05-31','2024-05-31'),
('R008', 'NONAC-Double', 'King', 2200.00, '2024-05-31','2024-05-31'),
('R009', 'AC-Suite', 'King', 3000.00, '2024-05-31','2024-05-31'),
('R010', 'AC-Suite', 'King', 3000.00, '2024-05-31','2024-05-31');

----------------------------------------------------------------------------------------------------------------

--Customer--
CREATE TABLE Customer (
    CustomerID VARCHAR(10) PRIMARY KEY,
    CusName VARCHAR(30),
	C_Address varchar(60) not null,
    CusNic varchar(13) not null,
    Cus_Tele char(10) not null,
    Cemail varchar(50) not null,
    CreateDate DATE,
	UpdateDate varchar(15) NULL
);

INSERT INTO Customer VALUES 
('C001','Dilsahn','Galle','922586089v','0775263522','sehei@gmail.com','2024-05-31','2024-05-31'),
('C002','Kamal','Kakirawa','822586089v','0714254569','sehani@gmail.com','2024-05-11','2024-05-31'),
('C003','Nimal','Kurunagala','962365089v','0725896458','sehelyi@gmail.com','2024-05-12','2024-05-31'),
('C004','Ravindu','Colombo3', '922486089v','0702485632','sehel@gmail.com','2024-05-13','2024-05-31'),
('C005','Pasan','Maradana','922167089v','0777145878','sehe@gmail.com','2024-05-14','2024-05-31');

----------------------------------------------------------------------------------------------------------------------------------------
DROP TABLE Employee;

--Employee--
CREATE TABLE Employee (
    EmployeeID VARCHAR(10) PRIMARY KEY,
    Name VARCHAR(30),
    Mobile varchar(12) not null,
    NIC char(15) not null,
    Address varchar(50) not null,
	Email varchar(50) not null,
	Designation varchar(50) not null,
    CreateDate DATE,
	UpdateDate DATE null
);

INSERT INTO Employee VALUES 
('E001','Dilsahn','0775263522','922586089v','Galle','sehelya@gmail.com','Chef','2024-05-31','2024-05-31'),
('E002','Kamal','0714254569','822586089v','Kakirawa','sehe@gmail.com','Hotel Manager','2024-05-11','2024-05-31'),
('E003','Nimal','0725896458','962365089v','Kurunegala','sehe@gmail.com','Waiter/Waitress','2024-05-12','2024-05-31'),
('E004','Ravindu','0702485632', '922486089v','Galle','seh@gmail.com','Security Officer','2024-05-13','2024-05-31'),
('E005','Pasan','0777145878','922167089v','Maradana','sehely@gmail.com','Maintenance Staff','2024-05-14','2024-05-31'),
('E006','Basiru','0777135878','922177089v','Maradana','seh@gmail.com','Admin','2024-05-14','2024-05-31'),
('E007','nasiru','0787135878','942177089v','Maradana','sehel@gmail.com','Cashier','2024-05-14','2024-05-31');

-------------------------------------------------------------------------------------------------------------------------------------------------------

--Inventory--

CREATE TABLE Inventory (
    InventoryID VARCHAR(10) PRIMARY KEY, 
    RoomID VARCHAR(10) NOT NULL,  
    InventoryName VARCHAR(50) NOT NULL,  
    InventoryType VARCHAR(50),  
    Quantity INT , 
    CreateDate DATE , 
    UpdatedDate DATE NULL, 
    FOREIGN KEY (RoomID) REFERENCES Room(RoomID)  
);

SELECT InventoryID,RoomID,InventoryName,InventoryType,Quantity,CreateDate,UpdatedDate FROM Inventory;
select * from Inventory;

INSERT INTO Inventory (InventoryID,RoomID, InventoryName, InventoryType, Quantity, CreateDate, UpdatedDate)
VALUES 
('I001','R001', 'Television', 'Electronics', 1, '2024-06-01','2024-06-01'),
('I002','R001', 'Queen bed', 'Furniture', 1, '2024-06-01', '2024-06-01'),
('I003','R002', 'King Bed', 'Furniture', 1, '2024-06-01','2024-06-01'),
('I004','R002', 'Arm chair', 'Furniture', 1, '2024-06-01','2024-06-01'),
('I005','R003', 'King Bed', 'Furniture', 1, '2024-06-01','2024-06-01');


--------------------------------------------------------------------------------------------------------------
Drop table reservation;

create table Reservation
(ReID varchar(10) primary key ,
Re_date Date,
check_in Date,
check_out Date,
Remarks varchar(20) null,
No_Person int,
CusID VARCHAR(10) Foreign Key  References Customer(CustomerID),
RoID VARCHAR(10) Foreign Key  References Room(RoomID));
select * from reservation;

INSERT INTO Reservation (ReID, Re_date, check_in, check_out, Remarks, No_Person, CusID, RoID)
VALUES 
('B001', '2024-06-01', '2024-06-05', '2024-06-10', 'Honeymoon', 2, 'C001', 'R005'),
('B002', '2024-06-02', '2024-06-06', '2024-06-09', 'Business Trip', 1, 'C002', 'R002'),
('B003', '2024-06-03', '2024-06-07', '2024-06-12', 'Vacation', 3, 'C003', 'R007'),
('B004', '2024-06-04', '2024-06-08', '2024-06-11', 'Family Stay', 4, 'C004', 'R009'),
('B005', '2024-06-05', '2024-06-09', '2024-06-13', 'Conference', 1, 'C005', 'R003');






