import api from "./apiService";

export const getUserFavorites = async (userId) => {
    const response = await api.get(`/Favorites/${userId}`);
    return response.data;
};

export const toggleFavorite = async (realEstateId) => {
    const response = await api.post(`/Favorites/toggle/${realEstateId}`);
    return response.data;
};