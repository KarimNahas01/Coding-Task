import './App.css';
import React, { useState, useEffect } from "react";

function App() {
  const [books, setBooks] = useState([]);
  const [status, setStatus] = useState("Checking backend...");
  const [backendAvailable, setBackendAvailable] = useState(false);

  useEffect(() => {
    fetch("http://localhost:5000/api/books")
      .then((res) => {
        if (!res.ok) {
          throw new Error("Backend is not available!");
        }
        return res.json();
      })
      .then((data) => {
        setBooks(data);
        setStatus("Books in the database:");
        setBackendAvailable(true);
      })
      .catch(() => {
        setStatus("Backend is not available!");
        setBackendAvailable(false);
      });
  }, []);
  
  return (
    <div className="App">
      <header className="App-header">
        <h2>{status}</h2>
        {backendAvailable ? (
          <div>
            <ul>
              {books.length > 0 ? (
                books.map((book) => (
                  <li key={book.id}>
                    <strong>{book.title}</strong> (ID: {book.id})
                  </li>
                ))
              ) : (
                <li>No books available</li>
              )}
            </ul>
          </div>
        ) : null}
      </header>
    </div>
  );
}

export default App;
