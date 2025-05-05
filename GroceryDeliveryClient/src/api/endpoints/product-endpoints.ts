export const ProductEndpoints = {
	getAll: {
		url: (params: string) => `/products${params ? `?${params}` : ""}` as const,
		key: (params: string) => ['products', params] as const
	},
	getById: {
		url: (id: number) => `/products/${String(id)}` as const,
		key: (id: number) => ['products', id] as const
	}
};