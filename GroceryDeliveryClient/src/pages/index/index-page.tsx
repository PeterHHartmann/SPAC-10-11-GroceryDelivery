import type { FC } from 'react';
import { Sidebar, SidebarContent, SidebarGroup, SidebarGroupContent, SidebarHeader, SidebarInset, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarProvider, SidebarRail } from '@/components/ui/sidebar';
import { useIsMobile } from '@/hooks/mobile';
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
		productId: 1,
		productName: 'Golden Delicious Apple',
		imagePath: null,
		price: 4,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stockQuantity: 0,
		categoryId: 1
	},
	{
		productId: 2,
		productName: 'Yoghurt',
		imagePath: null,
		price: 7,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stockQuantity: 2,
		categoryId: 1
	},
	{
		productId: 3,
		productName: 'Ground Beef',
		imagePath: null,
		price: 10,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stockQuantity: 10,
		categoryId: 1
	},
	{
		productId: 4,
		productName: 'Bartlett Pear',
		imagePath: null,
		price: 9,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stockQuantity: 0,
		categoryId: 1
	},
	{
		productId: 5,
		productName: 'Organic Banana',
		imagePath: null,
		price: 2,
		description: `
									Lorem ipsum dolor sit amet, 
									consectetur adipiscing elit. 
									Praesent nisi magna, sollicitudin ac congue eget, 
									pulvinar sed justo. In gravida iaculis elit, 
									vitae pretium nulla pharetra a.
									`,
		stockQuantity: 100,
		categoryId: 1

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
							<ProductCard key={`productcard-${String(product.productId)}-card-${String(index)}`} product={product} />
						))}
					</section>
				</SidebarInset>
				<SidebarRail />
			</SidebarProvider>
		</div>
	);
};