import axiosClient from "@/api/axiosClient";
import { OrderEndpoints } from "../endpoints/order-endpoints";
import { type Order } from "@/types";
import type { AxiosResponse } from 'axios';

/**
 * Service for handling product category-related API calls
 */
export const OrderService = {
	/**
	 * Fetch all product categories with optional sorting and ordering
	 * @returns Promise with all product category data
	 */
	async getAll(): Promise<Order[]> {
		const response = await axiosClient.get<string, AxiosResponse<Order[]>>(
			OrderEndpoints.getAll.url()
		);
		const { data } = response;
		return data;
	},

	/**
	 * Fetch a single product category by ID
	 * @param id - Category ID
	 * @returns Promise with product category data
	 */
	async getById(id: number): Promise<Order> {
		const response = await axiosClient.get<string, AxiosResponse<Order>>(OrderEndpoints.getById.url(id));
		const { data } = response;
		return data;
	},
    
    /**
     * Create a new product category
     * @param orderData - Order data to be created
     * @returns Promise with the created product category data
     */
    async create(orderData: Order): Promise<Order> {
        const response = await axiosClient.post<string, AxiosResponse<Order>>(OrderEndpoints.create.url(), orderData);
        const { data } = response;
        return data;
    },

    async update(id: number, orderData: Order): Promise<Order> {
        const response = await axiosClient.put<string, AxiosResponse<Order>>(OrderEndpoints.update.url(id), orderData);
        const { data } = response;
        return data;
    }
};
