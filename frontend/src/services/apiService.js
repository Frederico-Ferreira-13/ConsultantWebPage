import axios from 'axios';

const api = axios.create({ baseURL: `http://localhost:5170/api` });

api.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

api.interceptors.response.use(
    response => response,
    error => {
        if(error.response?.status === 401) {
            // O utilizador não está logado ou o token expirou
            window.location.href = `/login`;
        }
        return Promise.reject(error);
    }
);

export default api;