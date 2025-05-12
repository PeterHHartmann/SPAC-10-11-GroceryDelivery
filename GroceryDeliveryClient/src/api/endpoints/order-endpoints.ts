export const OrderEndpoints = {
	getAll: {
		url: () => `/order` as const,
		key: () => ['orders'] as const
	},
	getById: {
		url: (id: number) => `/order/${String(id)}` as const,
		key: (id: number) => ['order', id] as const
	},
	create: {
		url: () => `/order` as const,
		key: () => ['createOrder'] as const
	},
	update: {
		url: (id: number) => `/order/${String(id)}` as const,
		key: (id: number) => ['updateOrder', id] as const
	}
} as const;