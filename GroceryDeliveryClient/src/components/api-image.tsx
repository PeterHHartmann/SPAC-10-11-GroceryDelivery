import { FC, ComponentProps } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';

type ApiImageProps = {
	src: string | null;
} & Omit<ComponentProps<'img'>, 'src'>;

export const ApiImage: FC<ApiImageProps> = ({ src, ...props }) => {
	return (
		<img
			src={src ? src : PlaceholderImage}
			{...props}
		/>
	);
};