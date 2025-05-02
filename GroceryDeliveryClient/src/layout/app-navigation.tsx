import { Button } from '@/components/ui/button';
import { NavigationMenu, NavigationMenuItem, NavigationMenuLink, NavigationMenuList } from '@/components/ui/navigation-menu';
import { Separator } from '@/components/ui/separator';
import { ShoppingBasket, Store } from 'lucide-react';
import { Link } from 'react-router-dom';

export const AppNavigation: React.FC = () => {
	return (
		<header className='bg-background sticky flex inset-0 h-14 items-center gap-2 border-b w-full z-30'>
			<Link to={'/'} className='flex gap-2 p-2 items-center h-14 text-xl min-w-[16rem] border-r-1'>
				<Store size={24} />
				<h1>GROCERY DELIVERY</h1>
			</Link>
			<NavigationMenu className='max-w-full w-full justify-between z-30'>
				<NavigationMenuList>
					<NavigationMenuItem>
						<NavigationMenuLink asChild className='text-lg'>
							<Link to={'/'}>
								Shop
							</Link>
						</NavigationMenuLink>
					</NavigationMenuItem>
				</NavigationMenuList>
				<NavigationMenuList className='flex gap-2 ml-auto pr-4'>
					<NavigationMenuItem asChild>
						<NavigationMenuLink asChild>
							<Link to={'/cart'} className='flex gap-2 text-xl items-center'>
								<ShoppingBasket className='size-6' />
							</Link>
						</NavigationMenuLink>
					</NavigationMenuItem>
					<NavigationMenuItem className='h-8'>
						<Separator orientation='vertical' className='mx-2' />
					</NavigationMenuItem>
					<NavigationMenuItem asChild>
						<NavigationMenuLink asChild className='text-lg flex-row gap-2 justify-center items-center'>
							<Button size={'lg'} asChild>
								<Link to={'/login'}>
									<p>Sign in</p>
								</Link>
							</Button>
						</NavigationMenuLink>
					</NavigationMenuItem>
				</NavigationMenuList>
			</NavigationMenu>
		</header >
	);
};

