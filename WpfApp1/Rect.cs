using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static SDL2.SDL;

namespace WpfApp1
{
    public static class SdlRectMath
    {
        public static SDL_Rect UnlimitedRect = new SDL_Rect();
        public static SDL_Rect DummyEndResult = new SDL_Rect();
        public static SDL_Rect DummyEndResultAlternative = new SDL_Rect();

        static SdlRectMath()
        {
            UnlimitedRect.w = int.MaxValue;
            UnlimitedRect.h = int.MaxValue;
        }

        static public bool RayVsRect(float rayOriginX, float rayOriginY, float rayDirectionX, float rayDirectionY, ref SDL_Rect target, out float contactPointX, out float contactPointY, out int contactNormalX, out int contactNormalY, out float outHitNear)
        {

            float invertedRayX = 1.0f / rayDirectionX;
            float invertedRayY = 1.0f / rayDirectionY;

            float nearX = (target.x - rayOriginX) * invertedRayX;
            float nearY = (target.y - rayOriginY) * invertedRayY;

            float farX = (target.x + target.w - rayOriginX) * invertedRayX;
            float farY = (target.y + target.h - rayOriginY) * invertedRayY;

            if (nearX > farX)
            {
                (nearX, farX) = (farX, nearX);
            }

            if (nearY > farY)
            {
                (nearY, farY) = (farY, nearY);
            }

            if (nearX > farY || nearY > farX)
            {
                contactNormalX = 0;
                contactNormalY = 0;
                contactPointX = 0;
                contactPointY = 0;
                outHitNear = 0;
                return false;
            }

            float hitNear = Math.Max(nearX, nearY);
            float hitFar = Math.Min(farX, farY);

            outHitNear = hitNear;

            if (hitFar < 0)
            {
                contactNormalX = 0;
                contactNormalY = 0;
                contactPointX = 0;
                contactPointY = 0;
                outHitNear = 0;
                return false;
            }

            contactPointX = rayOriginX + hitNear * rayDirectionX;
            contactPointY = rayOriginY + hitNear * rayDirectionY;
            contactNormalX = 0;
            contactNormalY = 0;

            if (nearX > nearY)
            {
                if (rayDirectionX < 0)
                {
                    contactNormalX = 1;
                    contactNormalY = 0;
                }
                else
                {
                    contactNormalX = -1;
                    contactNormalY = 0;
                }
            }
            else if (nearX < nearY)
            {
                if (rayDirectionY < 0)
                {
                    contactNormalX = 0;
                    contactNormalY = -1;
                }
                else
                {
                    contactNormalX = 0;
                    contactNormalY = 1;
                }
            }

            return true;
        }

        static public void FromMinAndMax(Vector min, Vector max, out SDL_Rect outRect)
        {
            FromMinAndMax((float)min.X, (float)min.Y, (float)max.X, (float)max.Y, out outRect);
        }

        static public void FromMinAndMax(float minX, float minY, float maxX, float maxY, out SDL_Rect outRect)
        {
            bool bHasUserMistakenMinAndMax = minX > maxX && minY > maxY;

            if (bHasUserMistakenMinAndMax)
            {
                (maxX, minX) = (minX, maxX);
                (maxY, minY) = (minY, maxY);
            }

            outRect.x = (int)minX;
            outRect.y = (int)minY;
            outRect.w = (int)(maxX - minX);
            outRect.h = (int)(maxY - minY);
        }

        static public void FromMinAndMaxByRef(float minX, float minY, float maxX, float maxY, ref SDL_Rect outRect)
        {
            bool bHasUserMistakenMinAndMax = minX > maxX && minY > maxY;

            if (bHasUserMistakenMinAndMax)
            {
                (maxX, minX) = (minX, maxX);
                (maxY, minY) = (minY, maxY);
            }

            outRect.x = (int)minX;
            outRect.y = (int)minY;
            outRect.w = (int)(maxX - minX);
            outRect.h = (int)(maxY - minY);
        }

        static public void FromOriginAndExtend(float originX, float originY, float extendX, float extendY, out SDL_Rect outRect)
        {
            outRect.x = (int)(originX - extendX);
            outRect.y = (int)(originY - extendY);
            outRect.w = (int)(originX);
            outRect.h = (int)(originY);
        }

        static public void FromXywh(float x, float y, float w, float h, out SDL_Rect outRect)
        {
            outRect.x = (int)(x);
            outRect.y = (int)(y);
            outRect.w = (int)(w);
            outRect.h = (int)(h);
        }

        public static Vector PendicularVector(Vector v)
        {
            return new Vector(-v.Y, v.X);
        }

        public static bool IsPointInRect(ref SDL_Rect rectToTest, float x, float y)
        {
            return x >= rectToTest.x && y >= rectToTest.y && x <= rectToTest.x + rectToTest.w && y <= rectToTest.y + rectToTest.h;
        }
    }
}
