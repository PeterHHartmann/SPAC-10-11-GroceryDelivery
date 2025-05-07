import type { FC } from 'react';
import { Sidebar, SidebarContent, SidebarGroup, SidebarGroupContent, SidebarHeader, SidebarInset, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarProvider, SidebarRail } from '@/components/ui/sidebar';
import { useIsMobile } from '@/hooks/mobile';
import { useCategories } from '@/api/queries/category-queries';
import { RefreshCcw } from 'lucide-react';
import { Outlet } from 'react-router-dom';

export const ShopPage: FC = () => {
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
										<CategoriesMenu />
									</SidebarMenu>
								</SidebarGroupContent>
							</SidebarGroup>
						</SidebarContent>
					</Sidebar>
				</aside>
				<SidebarInset>
					<section className='w-full h-full p-4 grid grid-cols-3 grid-flow-row gap-2'>
						<Outlet />
					</section>
				</SidebarInset>
				<SidebarRail />
			</SidebarProvider>
		</div>
	);
};

const CategoriesMenu: FC = () => {
	const { data: categories, isLoading, error } = useCategories();

	if (isLoading) {
		return (
			<div className='w-full flex justify-center'>
				<RefreshCcw size={18} className='animate-spin' />
			</div>
		);
	}

	if (error || !categories) {
		return null;
	}

	return (
		<>
			{
				categories.map((category) => (
					<SidebarMenuItem key={'category-' + String(category.id)}>
						<SidebarMenuButton>
							{category.name}
						</SidebarMenuButton>
					</SidebarMenuItem>
				))
			}
		</>
	);
};