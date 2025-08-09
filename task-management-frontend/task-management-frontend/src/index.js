import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import '@progress/kendo-theme-default/dist/all.css';

// Suppress KendoReact license warnings
window.kendoLicenseActivation = false;
window.kendoLicenseKey = 'demo';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);