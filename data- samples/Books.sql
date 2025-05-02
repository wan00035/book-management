CREATE DATABASE Books;

USE Books;
CREATE TABLE books (
  BookID INT PRIMARY KEY AUTO_INCREMENT,
  Title VARCHAR(255),
  Author VARCHAR(255),
  PublicationYear DATE,
  IsCheckedOut BOOLEAN
);

INSERT INTO books (Title, Author, PublicationYear, IsCheckedOut)
VALUES ('To Kill a Mockingbird', 'Harper Lee', '1960-01-01', FALSE);

INSERT INTO books (Title, Author, PublicationYear, IsCheckedOut)
VALUES ('1984', 'George Orwell', '1949-01-01', TRUE);

INSERT INTO books (Title, Author, PublicationYear, IsCheckedOut)
VALUES ('The Great Gatsby', 'F. Scott Fitzgerald', '1925-01-01', FALSE);

INSERT INTO books (Title, Author, PublicationYear, IsCheckedOut)
VALUES ('Pride and Prejudice', 'Jane Austen', '1813-01-01', TRUE);

INSERT INTO books (Title, Author, PublicationYear, IsCheckedOut)
VALUES ('Moby Dick', 'Herman Melville', '1851-01-01', FALSE);

SELECT BookID, Title, Author, PublicationYear, IsCheckedOut FROM books;
SELECT * FROM books WHERE BookID = 3;
UPDATE books
SET Title = 'New Title', Author = 'New Author', PublicationYear = '2021-01-01', IsCheckedOut = TRUE
WHERE BookID = 1;
DELETE FROM books WHERE BookID = 3;

