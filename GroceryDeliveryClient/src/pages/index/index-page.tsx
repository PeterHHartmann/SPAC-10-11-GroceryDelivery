import type { FC } from 'react';
import { Sidebar, SidebarContent, SidebarGroup, SidebarGroupContent, SidebarHeader, SidebarInset, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarProvider, SidebarRail } from '@/components/ui/sidebar';
import { useIsMobile } from '@/hooks/use-mobile';

const categories = [
	'Fruit',
	'Vegetables',
	'Yoghurt',
	'Milk',
	'Meat'
];

export const IndexPage: FC = () => {
	const isMobile = useIsMobile();

	return (
		<div className='w-full h-full'>
			<SidebarProvider className='bg-slate-50'>
				<Sidebar collapsible={isMobile ? "offcanvas" : "none"} className='h-full z-0'>
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
				<SidebarInset>
					<div className='flex items-center justify-center w-full h-full bg-slate-100'>
						<h1 className='w-min'>Product Cards</h1>
					</div>
				</SidebarInset>
				<SidebarRail />
			</SidebarProvider>
		</div>
	);
};