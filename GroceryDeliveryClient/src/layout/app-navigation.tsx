import { Button } from '@/components/ui/button';
import { NavigationMenu, NavigationMenuItem, NavigationMenuLink, NavigationMenuList } from '@/components/ui/navigation-menu';
import { Separator } from '@/components/ui/separator';
import { useShoppingBasket } from '@/hooks/shopping-basket';
import { ShoppingBasket, Store } from 'lucide-react';
import { useMemo } from 'react';
import { Link } from 'react-router-dom';

export const AppNavigation: React.FC = () => {
	const { basket } = useShoppingBasket();

	const basketCount: number = useMemo(() => {
		return basket.reduce((total, item) => {
			return total + item.quantity;
		}, 0);
	}, [basket]);

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
					<NavigationMenuItem>
						<NavigationMenuLink asChild>
							<Link to={'/basket'} className='flex gap-2 text-xl items-center'>
								<div className='relative'>
									{basketCount > 0
										? <div className='absolute bg-destructive aspect-square min-w-[20px] w-max rounded-full top-[-10px] right-[-10px] flex justify-center items-center p-[2px]'>
											<p className='text-[12px] text-background font-bold'>
												{basketCount < 100
													? String(basketCount)
													: String(99) + '+'
												}
											</p>
										</div>
										: null
									}
									< ShoppingBasket className='size-6' />
								</div>
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

