import { NavigationMenu, NavigationMenuItem, NavigationMenuLink, NavigationMenuList } from '@/components/ui/navigation-menu';
import { Separator } from '@/components/ui/separator';
import { Bean, ShoppingCart } from 'lucide-react';
import { Link } from 'react-router-dom';

export const AppNavigation: React.FC = () => {
	return (
		<header className='sticky flex inset-0 h-14 shrink-0 items-center gap-2 border-b px-4 bg-slate-50 w-full z-30'>
			<NavigationMenu className='max-w-full justify-between'>
				<NavigationMenuList>
					<NavigationMenuItem asChild>
						<Link to={'/'} className='flex gap-2 text-xl items-center h-14 pr-7'>
							<Bean size={24} />
							<h1>GROCERY DELIVERY</h1>
						</Link>
					</NavigationMenuItem>
					<NavigationMenuItem className='h-14'>
						<Separator orientation='vertical' className='mr-4' />
					</NavigationMenuItem>
					<NavigationMenuItem>
						<NavigationMenuLink asChild className='text-lg'>
							<Link to={'/'}>
								Shop
							</Link>
						</NavigationMenuLink>
					</NavigationMenuItem>
				</NavigationMenuList>
				<NavigationMenuList className='flex gap-2 ml-auto'>
					<NavigationMenuItem asChild>
						<NavigationMenuLink asChild>
							<Link to={'/cart'} className='flex gap-2 text-xl items-center'>
								<ShoppingCart className='size-6' />
							</Link>
						</NavigationMenuLink>
					</NavigationMenuItem>
					<NavigationMenuItem className='h-8'>
						<Separator orientation='vertical' className='mx-2' />
					</NavigationMenuItem>
					<NavigationMenuItem asChild>
						<NavigationMenuLink asChild className='text-lg flex-row gap-2 justify-center items-center'>
							<Link to={'/login'}>
								<p>Sign in</p>
							</Link>
						</NavigationMenuLink>
					</NavigationMenuItem>
				</NavigationMenuList>
			</NavigationMenu>
		</header >
	);
};

