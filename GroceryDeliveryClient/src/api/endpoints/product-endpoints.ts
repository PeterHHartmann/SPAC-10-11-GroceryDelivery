export const ProductEndpoints = {
	getAll: {
		url: (params: string) => `/product${params ? `?${params}` : ""}` as const,
		key: (params: string) => ['products', params] as const
	},
	getById: {
		url: (id: number) => `/product/${String(id)}` as const,
		key: (id: number) => ['products', id] as const
	}
};