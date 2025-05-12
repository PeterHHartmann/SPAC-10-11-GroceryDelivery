import axiosClient from '@/api/axiosClient';
import { UserEndpoints } from '@/api/endpoints/user-endpoints';
import type { User } from '@/types';
import type { AxiosError, AxiosResponse } from 'axios';

/**
 * Service for handling user-related API calls
 */
export const UserService = {
	/**
	* Fetch all users
	* @returns Promise with all user data
	*/
	async getAll(): Promise<User[]> {
		const response = await axiosClient.get<string, AxiosResponse<User[], AxiosError>>(
			UserEndpoints.getAll.url()
		);
		const { data } = response;
		return data;
	},

	/**
	 * Fetch a single user by ID
	 * @param id - User ID
	 * @returns Promise with user data
	 */
	async getById(id: number): Promise<User> {
		const response = await axiosClient
			.get<string, AxiosResponse<User, AxiosError>>(
				UserEndpoints.getById.url(id)
			);
		const { data } = response;
		return data;
	},

};