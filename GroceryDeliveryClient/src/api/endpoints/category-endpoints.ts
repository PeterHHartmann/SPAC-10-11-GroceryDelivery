export const CategoryEndpoints = {
	getAll: {
		url: () => `/category` as const,
		key: () => ['categories'] as const
	},
	getById: {
		url: (id: number) => `/category/${String(id)}` as const,
		key: (id: number) => ['category', id] as const
	}
};