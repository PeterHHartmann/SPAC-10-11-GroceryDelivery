import { UserEndpoints } from '@/api/endpoints/user-endpoints';
import { UserService } from '@/api/services/user-service';
import type { User } from '@/types';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import type { AxiosError } from 'axios';

export const useUsers = (): UseQueryResult<User[], AxiosError> => {
	return useQuery<User[], AxiosError>({
		queryKey: [...UserEndpoints.getAll.key()],
		queryFn: () => UserService.getAll(),
	});
};

export const useUser = (id: number): UseQueryResult<User, AxiosError> => {
	return useQuery<User, AxiosError>({
		queryKey: [...UserEndpoints.getById.key(id)],
		queryFn: () => UserService.getById(id)
	});
};