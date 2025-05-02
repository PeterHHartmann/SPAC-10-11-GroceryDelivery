export type Product = {
	id: string | number;
	name: string;
	image: string | null;
	price: number;
	description?: string;
	stock?: number;
};