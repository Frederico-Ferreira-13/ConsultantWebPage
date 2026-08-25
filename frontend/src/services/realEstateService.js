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

export const getPropertyMedia = async (id) => {
    const response = await api.get(`/RealEstateMedia/property/${id}`);
    return response.data;
};

export const getPropertyCategories = async (id) => {
    const response = await api.get(`/RealEstate/${id}/categories`);
    return response.data;
};

export const getPropertyComments = async (id) => {
    const response = await api.get(`/RealEstate/${id}/comments`);
    return response.data;
};

export const addPropertyComment = async (id, commentText) => {
    const response = await api.post(`/RealEstate/${id}/comments`, { commentText });
    return response.data;
};

export const requestPropertyInformation = async (id, { clientName, clientEmail, message }) => {
    const response = await api.post(`/RealEstate/${id}/information`, {
        clientName,
        clientEmail,
        message,
    });
    return response.data;
};

export const scheduleVisit = async (id, preferredDateIso) => {
    const response = await api.post(`/RealEstate/${id}/visit`, {
        preferredDate: preferredDateIso,
    });
    return response.data;
};