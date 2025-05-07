export const ProductEndpoints = {
	getAll: {
		url: (params: string) => `/product${params ? `?${params}` : ""}`,
		key: (params: string) => ['products', params]
	},
	getById: {
		url: (id: number) => `/product/${String(id)}`,
		key: (id: number) => ['products', id]
	}
} as const;