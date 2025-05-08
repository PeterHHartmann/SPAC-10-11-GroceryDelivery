import { useMemo, type FC } from 'react';
import { Sidebar, SidebarContent, SidebarGroup, SidebarGroupContent, SidebarHeader, SidebarInset, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarProvider, SidebarRail } from '@/components/ui/sidebar';
import { useIsMobile } from '@/hooks/mobile';
import { useCategories } from '@/api/queries/category-queries';
import { RefreshCcw } from 'lucide-react';
import { Outlet, useSearchParams } from 'react-router-dom';

export const ShopPage: FC = () => {
	const isMobile = useIsMobile();
	const [searchParams, setSearchParams] = useSearchParams();

	const handleSelectCategory = (categoryId: number): void => {
		const params = searchParams;
		params.set('categoryId', String(categoryId));
		params.set('page', '1');
		setSearchParams(params, {
			preventScrollReset: true
		});
		return;
	};

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
										<CategoriesMenu categorySelectedHandler={handleSelectCategory} />
									</SidebarMenu>
								</SidebarGroupContent>
							</SidebarGroup>
						</SidebarContent>
					</Sidebar>
				</aside>
				<SidebarInset>
					<Outlet />
				</SidebarInset>
				<SidebarRail />
			</SidebarProvider>
		</div>
	);
};

type CategoriesMenuProps = {
	categorySelectedHandler: (categoryId: number) => void;
};

const CategoriesMenu: FC<CategoriesMenuProps> = ({ categorySelectedHandler }) => {
	const { data: categories, isLoading, error } = useCategories();
	const [searchParams] = useSearchParams();
	const selectedCategory: string | null = useMemo(() => {
		return searchParams.get('categoryId');
	}, [searchParams]);

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
						{/* TODO implement active state */}
						<SidebarMenuButton
							onClick={() => { categorySelectedHandler(category.id); }}
							isActive={selectedCategory ? String(category.id) === selectedCategory : false}
						>
							{category.name}
						</SidebarMenuButton>
					</SidebarMenuItem>
				))
			}
		</>
	);
};