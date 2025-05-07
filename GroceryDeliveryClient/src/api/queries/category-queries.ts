import { useQuery } from '@tanstack/react-query';
import { CategoryEndpoints } from '@/api/endpoints/category-endpoints';
import { CategoryService } from '@/api/services/category-service';

export const useCategories = () => {
	return useQuery({
		queryKey: [CategoryEndpoints.getAll.key()],
		queryFn: () => CategoryService.getAll(),
	});
};

export const useCategory = (id: number) => {
	return useQuery({
		queryKey: [CategoryEndpoints.getById.key(id)],
		queryFn: () => CategoryService.getById(id)
	});
};