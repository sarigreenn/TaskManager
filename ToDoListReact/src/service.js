import axios from 'axios';

// הגדרת כתובת ה-API כ-default באמצעות Config Defaults
axios.defaults.baseURL = "http://localhost:5120";

// הוספת interceptor לתפיסת שגיאות ב-response ורישום ללוג
axios.interceptors.response.use(
  response => response,
  error => {
    console.error('Error response:', error.response);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get('/api/items');    
    return result.data;
  },

  addTask: async (name) => {
    console.log('addTask', name)
    const result = await axios.post('/api/items', { name });
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', {id, isComplete})
    const result = await axios.put(`/api/items/${id}`, { isComplete });
    return result.data;
  },

  deleteTask: async (id) => {
    console.log('deleteTask')
    const result = await axios.delete(`/api/items/${id}`);
    return result.data;
  }
};
