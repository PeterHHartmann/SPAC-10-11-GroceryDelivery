import { OrderEndpoints } from '../endpoints/order-endpoints';
import { OrderService } from '../services/order-service';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { Order } from '@/types';


export const useOrders = () => {
	return useQuery({
		queryKey: [OrderEndpoints.getAll.key()],
		queryFn: () => OrderService.getAll(),
	});
};

export const useOrder = (id: number) => {
	return useQuery({
		queryKey: [OrderEndpoints.getById.key(id)],
		queryFn: () => OrderService.getById(id)
	});
};

export const useCreateOrder = () => {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: (orderData: Order) => OrderService.create(orderData),
        onSuccess: async () => {
            // Invalidate and refetch orders query after a successful creation
            try {
                await queryClient.invalidateQueries({ queryKey: [OrderEndpoints.getAll.key()] });
            } catch (error) {
                console.error("Failed to invalidate queries:", error);
                // Optionally, handle the error further (e.g., show a notification to the user)
            }
        },
    });
};

export const useUpdateOrder = (id: number, orderData: Order) => {
    return useQuery({
        queryKey: [OrderEndpoints.update.key(id)],
        queryFn: () => OrderService.update(id, orderData),
    });
}