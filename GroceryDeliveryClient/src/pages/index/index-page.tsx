import type { FC } from 'react';
import { Sidebar, SidebarContent, SidebarGroup, SidebarGroupContent, SidebarHeader, SidebarInset, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarProvider, SidebarRail } from '@/components/ui/sidebar';
import { useIsMobile } from '@/hooks/use-mobile';
import { ProductCard } from '@/components/product-card';
import type { Product } from '@/types';

const categories = [
	'Fruit',
	'Vegetables',
	'Yoghurt',
	'Milk',
	'Meat'
];

const placeholderProducts: Product[] = [
	{
		id: 1,
		name: 'Golden Delicious Apple',
		image: null,
		price: 4,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stock: 0

	},
	{
		id: 2,
		name: 'Yoghurt',
		image: null,
		price: 7,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stock: 2

	},
	{
		id: 3,
		name: 'Ground Beef',
		image: null,
		price: 10,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stock: 10

	},
	{
		id: 4,
		name: 'Bartlett Pear',
		image: null,
		price: 9,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stock: 0

	},
	{
		id: 5,
		name: 'Organic Banana',
		image: null,
		price: 2,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stock: 100

	}
];

export const IndexPage: FC = () => {
	const isMobile = useIsMobile();

	return (
		<div className='w-full h-full'>
			<SidebarProvider>
				<aside>
					<Sidebar collapsible={isMobile ? "offcanvas" : "none"} className='h-full z-0 border-r-1'>
						<SidebarHeader>
							<h1 className='text-xl'>Categories</h1>
						</SidebarHeader>
						<SidebarContent>
							<SidebarGroup>
								<SidebarGroupContent>
									<SidebarMenu>
										{categories.map((category) => (
											<SidebarMenuItem key={category}>
												<SidebarMenuButton>
													{category}
												</SidebarMenuButton>
											</SidebarMenuItem>
										))}
									</SidebarMenu>
								</SidebarGroupContent>
							</SidebarGroup>
						</SidebarContent>
					</Sidebar>
				</aside>
				<SidebarInset>
					<section className='w-full h-full p-4 grid grid-cols-3 grid-flow-row gap-2'>
						{placeholderProducts.map((product, index) => (
							<ProductCard key={`product-${product.id}-card-${index}`} product={product} />
						))}
					</section>
				</SidebarInset>
				<SidebarRail />
			</SidebarProvider>
		</div>
	);
};