
CREATE TABLE IF NOT EXISTS Books (
  BookID INT PRIMARY KEY AUTO_INCREMENT,
  Title VARCHAR(255),
  Author VARCHAR(255),
  PublicationYear DATE,
  IsCheckedOut BOOLEAN
);


INSERT INTO Books (Title, Author, PublicationYear, IsCheckedOut)
VALUES 
('To Kill a Mockingbird', 'Harper Lee', '1960-01-01', FALSE),
('1984', 'George Orwell', '1949-01-01', TRUE),
('The Great Gatsby', 'F. Scott Fitzgerald', '1925-01-01', FALSE),
('Pride and Prejudice', 'Jane Austen', '1813-01-01', TRUE),
('Moby Dick', 'Herman Melville', '1851-01-01', FALSE);


CREATE TABLE IF NOT EXISTS Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(50) NOT NULL,
    Role VARCHAR(20) NOT NULL
);


INSERT INTO Users (Username, Password, Role) VALUES ('admin', '123', 'Admin');
INSERT INTO Users (Username, Password, Role) VALUES ('reader', '123', 'User');


CREATE TABLE IF NOT EXISTS BorrowRecords (
    RecordID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    BookID INT NOT NULL,
    BorrowDate DATETIME NOT NULL,         
    DueDate DATETIME NOT NULL,             
    ReturnDate DATETIME NULL,             
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (BookID) REFERENCES Books(BookID)
);

INSERT INTO BorrowRecords (UserID, BookID, BorrowDate, DueDate, ReturnDate) 
VALUES (1, 2, NOW(), DATE_ADD(NOW(), INTERVAL 14 DAY), NULL);