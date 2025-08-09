import React, { useState } from 'react';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { useNavigate } from 'react-router-dom';
import { login } from '../../services/authService';
import './style.css'; // Don't forget to create and link the CSS file.

function LoginForm() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const data = await login(email, password);

      // Store the entire response in localStorage (customize as needed)
      localStorage.setItem('user', JSON.stringify(data));

      // Show success toast immediately after successful API response
      toast.success('Login successful!', {
        position: 'top-right',
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
        icon: false,
        closeButton: false
      });

      // Extract role from API response
      const userRole = data.value?.user?.roles?.[0];

      // Route based on role from API response with a small delay to show toast
      setTimeout(() => {
        if (userRole === 'Employee') {
          navigate('/employee-dashboard');
        } else if (userRole === 'Manager') {
          navigate('/manager-dashboard');
        } else {
          toast.error(`Unknown user role: ${userRole}. Please contact support.`, {
            position: 'top-right',
            autoClose: 5000,
            icon: false,
            closeButton: false
          });
        }
      }, 1000); // 1 second delay to ensure toast is visible
    } catch (error) {
      toast.error(error.message, {
        position: 'top-right',
        autoClose: 5000,
        icon: false,
        closeButton: false
      });
    }
  };

  return (
    <div className="login-container">
      <div className="login-form">
        <h2>Login to Your Account</h2>
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <label>Email</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              placeholder="Enter your email"
            />
          </div>

          <div className="input-group">
            <label>Password</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              placeholder="Enter your password"
            />
          </div>

          <button type="submit" className="login-button">Login</button>
        </form>
      </div>
      <ToastContainer
        position="top-right"
        autoClose={3000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
        toastStyle={{
          backgroundColor: '#4caf50',
          color: 'white',
          fontSize: '14px',
          fontWeight: '500'
        }}
        closeButton={false}
        icon={false}
      />
    </div>
  );
}

export default LoginForm;
