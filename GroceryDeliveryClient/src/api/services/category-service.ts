import axiosClient from "@/api/axiosClient";
import { CategoryEndpoints } from '@/api/endpoints/category-endpoints';
import { type Category } from "@/types";
import type { AxiosResponse } from 'axios';

/**
 * Service for handling product category-related API calls
 */
export const CategoryService = {
	/**
	 * Fetch all product categories with optional sorting and ordering
	 * @returns Promise with all product category data
	 */
	async getAll(): Promise<Category[]> {
		const response = await axiosClient.get<string, AxiosResponse<Category[]>>(
			CategoryEndpoints.getAll.url()
		);
		const { data } = response;
		return data;
	},

	/**
	 * Fetch a single product category by ID
	 * @param id - Category ID
	 * @returns Promise with product category data
	 */
	async getById(id: number): Promise<Category> {
		const response = await axiosClient.get<string, AxiosResponse<Category>>(CategoryEndpoints.getById.url(id));
		const { data } = response;
		return data;
	}
};
