import { AppNavigation } from '@/layout/app-navigation';
import { Outlet } from 'react-router-dom';


export const RootLayout: React.FC = () => {
	return (
		<>
			<AppNavigation />
			<header>
			</header>
			<main>
				<Outlet />
			</main>
		</>
	);
};