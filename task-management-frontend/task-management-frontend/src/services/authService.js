const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

/**
 * Check if user is authenticated and token is valid
 * @returns {boolean} True if user is authenticated with valid token
 */
export function isAuthenticated() {
  try {
    const user = JSON.parse(localStorage.getItem('user'));
    if (!user || !user.value || !user.value.token) {
      return false;
    }

    // Check if token is expired
    try {
      const payload = JSON.parse(atob(user.value.token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);
      return payload.exp && payload.exp > currentTime;
    } catch {
      return false;
    }
  } catch {
    return false;
  }
}

/**
 * Clear user data and redirect to login
 */
export function logout() {
  localStorage.removeItem('user');
  window.location.href = '/';
}

/**
 * Login user with email and password.
 * @param {string} email
 * @param {string} password
 * @returns {Promise<object>} Response data if successful.
 * @throws {Error} Throws error with message from API or network.
 */
export async function login(email, password) {
  try {
    const response = await fetch(`${API_BASE_URL}/Auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      let errorMsg = 'Login failed. Please try again.';
      try {
        const errorData = await response.json();
        errorMsg = errorData.message || errorMsg;
      } catch (parseError) {
        // If response is not JSON, keep default errorMsg
      }
      throw new Error(errorMsg);
    }

    const responseData = await response.json();
    return responseData;
  } catch (error) {
    // Network or other error
    throw new Error(error.message || 'Network error. Please try again later.');
  }
}
