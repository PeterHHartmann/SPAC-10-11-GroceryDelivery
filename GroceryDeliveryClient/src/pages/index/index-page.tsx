import type { FC } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';

export const IndexPage: FC = () => {
	return (
		<div className='flex w-full'>
			<img src={PlaceholderImage} className="logo" alt="Image placeholder" />
		</div>
	);
};