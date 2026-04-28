import { api, handleApiResponse } from "../lib/api";

const ITEMS_API = "/api/items";

export const itemService = {

    async getItems(params = {}) {
        const query = new URLSearchParams({
            skip: params.skip ?? 0,
            take: params.take ?? 20,
            ...(params.search && { search: params.search })
        });

        const response = await api(`${ITEMS_API}?${query}`);
        return handleApiResponse(response);
    },

    async getItem(id) {
        const response = await api(`${ITEMS_API}/${id}`);
        return handleApiResponse(response);
    },

    async createItem(item) {
        const response = await api(ITEMS_API, {
            method: "POST",
            body: JSON.stringify(item)
        });
        return handleApiResponse(response);
    },

    async updateItem(id, item) {
        const response = await api(`${ITEMS_API}/${id}`, {
            method: "PUT",
            body: JSON.stringify(item)
        });
        return handleApiResponse(response);
    },

    async patchItem(id, updates) {
        const response = await api(`${ITEMS_API}/${id}`, {
            method: "PATCH",
            body: JSON.stringify(updates)
        });
        return handleApiResponse(response);
    },

    async deleteItem(id) {
        const response = await api(`${ITEMS_API}/${id}`, {
            method: "DELETE"
        });
        return handleApiResponse(response);
    }
};
