
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';
import LoginPage from './pages/LoginPage';
import EmployeePage from './pages/EmployeePage';
import ManagerPage from './pages/ManagerPage';

function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<LoginPage />} />
          <Route path="/employee-dashboard" element={<EmployeePage />} />
          <Route path="/manager-dashboard" element={<ManagerPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
