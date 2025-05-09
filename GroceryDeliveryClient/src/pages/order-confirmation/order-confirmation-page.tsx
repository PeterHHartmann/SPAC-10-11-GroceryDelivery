import { Button } from '@/components/ui/button';
import { useShoppingBasket } from '@/hooks/shopping-basket';
import { CheckCircle } from 'lucide-react';
import { useEffect, type FC } from 'react';
import { Link } from 'react-router-dom';

export const OrderConfirmationPage: FC = () => {
    const { clearBasket } = useShoppingBasket();

    useEffect(() => {
        clearBasket();
        return;
    }, [clearBasket]);

    return (
        <div className="container mx-auto p-4 flex flex-col items-center justify-center min-h-[calc(100vh-200px)]">
            <CheckCircle className="w-16 h-16 text-green-500 mb-6" />
            <h1 className="text-3xl font-bold mb-4 text-center">Thank You for Your Order!</h1>
            <p className="text-lg text-gray-700 mb-8 text-center">
                Your order has been successfully placed. You will receive an email confirmation shortly.
            </p>
            <div className="flex space-x-4">
                <Button asChild>
                    <Link to="/">Go to Homepage</Link>
                </Button>

            </div>
        </div>
    );
};