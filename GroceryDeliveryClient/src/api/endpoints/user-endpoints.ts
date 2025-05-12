export const UserEndpoints = {
	getAll: {
		url: () => `/user`,
		key: () => ['users']
	},
	getById: {
		url: (id: number) => `/user/${String(id)}`,
		key: (id: number) => ['users', id]
	}
} as const;