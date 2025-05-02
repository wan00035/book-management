
CREATE TABLE books (
  BookID INT PRIMARY KEY AUTO_INCREMENT,
  Title VARCHAR(255),
  Author VARCHAR(255),
  PublicationYear DATE,
  IsCheckedOut BOOLEAN
);

INSERT INTO books (Title, Author, PublicationYear, IsCheckedOut)
VALUES 
('To Kill a Mockingbird', 'Harper Lee', '1960-01-01', FALSE),
('1984', 'George Orwell', '1949-01-01', TRUE),
('The Great Gatsby', 'F. Scott Fitzgerald', '1925-01-01', FALSE),
('Pride and Prejudice', 'Jane Austen', '1813-01-01', TRUE),
('Moby Dick', 'Herman Melville', '1851-01-01', FALSE);