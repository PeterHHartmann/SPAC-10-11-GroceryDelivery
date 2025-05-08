import { FC, ComponentProps } from 'react';
import PlaceholderImage from '@/assets/placeholder-image.svg';
import { API_URL } from '@/api/axiosClient';

type ApiImageProps = {
	src: string | null;
} & Omit<ComponentProps<'img'>, 'src'>;

export const ApiImage: FC<ApiImageProps> = ({ src, ...props }) => {
	return (
		<img
			src={src ? API_URL + src : PlaceholderImage}
			{...props}
		/>
	);
};