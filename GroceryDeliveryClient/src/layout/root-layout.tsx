import { AppNavigation } from '@/layout/app-navigation';
import { Outlet } from 'react-router-dom';

export const RootLayout: React.FC = () => (
	<>
		<AppNavigation />
		<main className='flex flex-1 w-full p-8 bg-white min-h-[calc(100vh-64px+600px)]'>
			<Outlet />
		</main>
	</>
);