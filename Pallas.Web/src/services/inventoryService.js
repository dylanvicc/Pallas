import { api, handleApiResponse } from "../lib/api";

const INVENTORY_API = "/api/inventory";

export const inventoryService = {

    async getInventory(params = {}) {
        const query = new URLSearchParams({
            skip: params.skip ?? 0,
            take: params.take ?? 20
        });

        const response = await api(`${INVENTORY_API}?${query}`);
        return handleApiResponse(response);
    },

    async getItem(id) {
        const response = await api(`${INVENTORY_API}/${id}`);
        return handleApiResponse(response);
    },

    async createItem(item) {
        const response = await api(INVENTORY_API, {
            method: "POST",
            body: JSON.stringify(item)
        });
        return handleApiResponse(response);
    },

    async updateItem(id, item) {
        const response = await api(`${INVENTORY_API}/${id}`, {
            method: "PUT",
            body: JSON.stringify(item)
        });
        return handleApiResponse(response);
    },

    async deleteItem(id) {
        const response = await api(`${INVENTORY_API}/${id}`, {
            method: "DELETE"
        });
        return handleApiResponse(response);
    }
};
