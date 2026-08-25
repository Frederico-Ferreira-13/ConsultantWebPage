import api from "./apiService";

export const getAverageRating = async (realEstateId) => {
    const response = await api.get(`/Ratings/property/${realEstateId}/average`);
    return response.data;
};

export const getUserRating = async (realEstateId, userId) => {
    try {
        const response = await api.get(`/Ratings/property/${realEstateId}/user/${userId}`);
        return response.data;
    } catch {
        return null;
    }
};

export const addOrUpdateRating = async (realEsatteId, stars) => {
    const response = await api.post('/Ratings', { realEsatteId, stars });
    return response.data;
};