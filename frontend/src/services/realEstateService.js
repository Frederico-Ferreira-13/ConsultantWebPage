import api from './apiService.js';

export const getProperties = async () => {
    const response = await api.get('/RealEstate');
    return response.data;
};

export const getPropertyById = async (id) => {
    const response = await api.get(`/RealEstate/${id}`);
    return response.data;
};

export const searchProperties = async ({ city, maxPrice, rooms } = {}) => {
    const params = {};
    if (city) params.city = city;
    if (maxPrice) params.maxPrice = maxPrice;
    if (rooms) params.rooms = rooms;

    const response = await api.get('/RealEstate/search', { params });
    return response.data;
};

export const getPropertiesByCategory = async (categoryId) => {
    const response = await api.get(`/RealEstate/category/${categoryId}`);
    return response.data;
};