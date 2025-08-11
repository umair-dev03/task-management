const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

/**
 * Get JWT token from localStorage
 * @returns {string|null} JWT token or null if not found
 */
function getAuthToken() {
  try {
    const user = JSON.parse(localStorage.getItem('user'));
    // Based on API response structure: { isSuccess: true, value: { user: {...}, token: "..." } }
    const token = user && user.value && user.value.token ? user.value.token : null;

    if (!token) {
      return null;
    }

    // Check if token is expired by decoding it
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const currentTime = Math.floor(Date.now() / 1000);

      if (payload.exp && payload.exp < currentTime) {
        // Clear expired token and redirect to login
        localStorage.removeItem('user');
        window.location.href = '/';
        return null;
      }
    } catch (decodeError) {
      return null;
    }

    return token;
  } catch (error) {
    return null;
  }
}

/**
 * Create a new task.
 * @param {Object} taskData - { title: string, date: string (ISO), hourWorked: number }
 * @returns {Promise<object>} Response data if successful.
 * @throws {Error} Throws error with message from API or network.
 */
export async function createTask(taskData) {
  try {
    const token = getAuthToken();

    const response = await fetch(`${API_BASE_URL}/Task`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify(taskData),
    });

    if (!response.ok) {
      if (response.status === 401) {
        // Token is invalid or expired, redirect to login
        localStorage.removeItem('user');
        window.location.href = '/';
        throw new Error('Authentication failed. Please login again.');
      }

      let errorMsg = 'Failed to create task. Please try again.';
      try {
        const errorData = await response.json();
        errorMsg = errorData.message || errorMsg;
      } catch {
        // If response is not JSON, keep default errorMsg
      }
      throw new Error(errorMsg);
    }

    return await response.json();
  } catch (error) {
    throw new Error(error.message || 'Network error. Please try again later.');
  }
}

/**
 * Fetch paginated tasks with optional search and status filter.
 * @param {Object} params - { page, pageSize, searchEmployeeName, status }
 * @returns {Promise<object>} API response with items, totalCount, pageNumber, pageSize.
 */
export async function fetchTasksPaginated({ page = 1, pageSize = 10, searchEmployeeName = '', status = '' } = {}) {
  try {
    const token = getAuthToken();

    const query = new URLSearchParams();
    query.append('page', page);
    query.append('pageSize', pageSize);
    if (searchEmployeeName) query.append('title', searchEmployeeName);
    if (status) query.append('status', status);
    // Add sort parameter to get newest tasks first
    query.append('sortBy', 'date');
    query.append('sortOrder', 'desc');

    const url = `${API_BASE_URL}/Task?${query.toString()}`;

    const headers = {
      'Content-Type': 'application/json',
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(url, {
      method: 'GET',
      headers,
    });



    if (!response.ok) {
      if (response.status === 401) {
        // Token is invalid or expired, redirect to login
        localStorage.removeItem('user');
        window.location.href = '/';
        throw new Error('Authentication failed. Please login again.');
      }

      let errorMsg = 'Failed to fetch tasks. Please try again.';
      try {
        const errorData = await response.json();
        errorMsg = errorData.message || errorMsg;
      } catch {
        // If response is not JSON, keep default errorMsg
      }
      throw new Error(errorMsg);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    throw new Error(error.message || 'Network error. Please try again later.');
  }
}

/**
 * Update an existing task.
 * @param {number} taskId - The ID of the task to update
 * @param {Object} taskData - { title: string, date: string (ISO), hourWorked: number, status: string }
 * @returns {Promise<object>} Response data if successful.
 * @throws {Error} Throws error with message from API or network.
 */
export async function updateTask(taskId, taskData) {
  try {
    const token = getAuthToken();
    const url = `${API_BASE_URL}/Task/${taskId}`;



    const response = await fetch(url, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify(taskData),
    });

    if (!response.ok) {
      if (response.status === 401) {
        // Token is invalid or expired, redirect to login
        localStorage.removeItem('user');
        window.location.href = '/';
        throw new Error('Authentication failed. Please login again.');
      }

      let errorMsg = 'Failed to update task. Please try again.';
      try {
        const errorData = await response.json();
        errorMsg = errorData.message || errorMsg;
      } catch {
        // If response is not JSON, keep default errorMsg
      }
      throw new Error(errorMsg);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    throw new Error(error.message || 'Network error. Please try again later.');
  }
}

export async function updateTaskStatus(taskId, status) {
  try {
    const token = getAuthToken();
    const url = `${API_BASE_URL}/Task/${taskId}/status`;

    const response = await fetch(url, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      body: JSON.stringify({ status }),
    });

    if (!response.ok) {
      if (response.status === 401) {
        // Token is invalid or expired, redirect to login
        localStorage.removeItem('user');
        window.location.href = '/';
        throw new Error('Authentication failed. Please login again.');
      }

      let errorMsg = 'Failed to update task status. Please try again.';
      try {
        const errorData = await response.json();
        errorMsg = errorData.message || errorMsg;
      } catch {
        // If response is not JSON, keep default errorMsg
      }
      throw new Error(errorMsg);
    }

    return await response.json();
  } catch (error) {
    throw new Error(error.message || 'Network error. Please try again later.');
  }
}

/**
 * Delete a task.
 * @param {number} taskId - The ID of the task to delete
 * @returns {Promise<boolean>} Returns true if successful.
 * @throws {Error} Throws error with message from API or network.
 */
export async function deleteTask(taskId) {
  try {
    const token = getAuthToken();
    const url = `${API_BASE_URL}/Task/${taskId}`;



    const response = await fetch(url, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
    });

    if (!response.ok) {
      if (response.status === 401) {
        // Token is invalid or expired, redirect to login
        localStorage.removeItem('user');
        window.location.href = '/';
        throw new Error('Authentication failed. Please login again.');
      }

      let errorMsg = 'Failed to delete task. Please try again.';
      try {
        const errorData = await response.json();
        errorMsg = errorData.message || errorMsg;
      } catch {
        // If response is not JSON, keep default errorMsg
      }
      throw new Error(errorMsg);
    }

    // DELETE API returns 204 with no content, so we don't try to parse JSON
    return true;
  } catch (error) {
    throw new Error(error.message || 'Network error. Please try again later.');
  }
}
