import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { MinusCircle, PlusCircle } from 'lucide-react';
import { type FC } from 'react';

type StepperProps = {
	count: number,
	min: number,
	max?: number,
	minusPressedHandler: () => void;
	plusPressedHandler: () => void;
};

export const Stepper: FC<StepperProps> = ({ count, max, min, minusPressedHandler, plusPressedHandler }) => {

	if (!max || max === 0) {
		return null;
	}

	return (
		<div className='flex gap-1'>
			<Button
				size={'icon'}
				onClick={minusPressedHandler}
				disabled={count <= min}
			>
				<MinusCircle size={8} />
			</Button>
			<Input value={count} type={'number'} className='stepper appearance-none max-w-12' />
			<Button
				size={'icon'}
				onClick={plusPressedHandler}
				disabled={count === max}
			>
				<PlusCircle size={8} />
			</Button>
		</div>
	);
};