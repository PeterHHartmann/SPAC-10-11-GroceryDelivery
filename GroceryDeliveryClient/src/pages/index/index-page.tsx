import type { FC } from 'react';
import NoImageIcon from '@/assets/no-image.svg';

export const IndexPage: FC = () => {
	return (
		<div className='flex w-full'>
			<img src={NoImageIcon} className="logo" alt="Image placeholder" />
		</div>
	);
};